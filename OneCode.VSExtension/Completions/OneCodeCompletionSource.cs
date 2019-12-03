using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using OneCode.Core;

namespace OneCode.VsExtension.Completions
{
    public class OneCodeCompletionSource : IAsyncCompletionSource
    {
        private static ImageElement CompletionItemIcon { get; } = new ImageElement(new ImageId(new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369"), KnownImageIds.AddMethod), "Add Method Icon");
        private ImmutableArray<CompletionItem>? Items { get; set; }
        private ImmutableArray<CompletionFilter>? Filters { get; set; }
        private ImmutableArray<ImageElement>? Images { get; set; }
        private Repositories Repositories { get; set; }

        public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token)
        {
            // Since we are plugging in to CSharp content type,
            // allow the CSharp language service to pick the Applicable To Span.
            return CompletionStartData.ParticipatesInCompletionIfAny;
            // Alternatively, we've got to provide location for completion
            // return new CompletionStartData(CompletionParticipation.ProvidesItems, ...
        }

        public async Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
        {
            session.Properties["LineNumber"] = triggerLocation.GetContainingLine().LineNumber;

            Repositories ??= await OneCodePackage.Repositories.GetValueAsync(token);
            Filters ??= ImmutableArray.Create<CompletionFilter>();
            Images ??= ImmutableArray.Create<ImageElement>();
            Items ??= ImmutableArray.Create(Repositories.Values
                .Select(repository => repository.Files
                    .Select(file => file.Code.Classes.Select(@class => @class.Methods
                            .Where(method => method.IsStatic)
                            .Select(method =>
                            {
                                var item = new CompletionItem(
                                    $"{@class.Name}.{method.Name}",
                                    this,
                                    CompletionItemIcon,
                                    Filters.Value,
                                    file.Code.NamespaceName,
                                    $"{@class.Name}.{method.Name.Substring(0, method.Name.IndexOf('(') + 1)}",
                                    $"{@class.Name}.{method.Name}",
                                    $"{@class.Name}.{method.Name}",
                                    Images.Value);

                                item.Properties[nameof(Repository)] = repository;
                                item.Properties[nameof(CodeFile)] = file;
                                item.Properties[nameof(Class)] = @class;
                                item.Properties[nameof(Method)] = method;

                                return item;
                            }))
                        .SelectMany(i => i))
                    .SelectMany(i => i))
                .SelectMany(i => i)
                .ToArray());

            return new CompletionContext(Items.Value);
        }

        public Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token)
        {
            var content = new ContainerElement(
                ContainerElementStyle.Wrapped,
                CompletionItemIcon,
                new ClassifiedTextElement(
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.Keyword, "Hello!"),
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.Identifier, " This is a sample item")));
            var lineInfo = new ClassifiedTextElement(
                    new ClassifiedTextRun(
                        PredefinedClassificationTypeNames.Comment,
                        "You are on line " + ((int)(session.Properties["LineNumber"]) + 1)));
            var timeInfo = new ClassifiedTextElement(
                    new ClassifiedTextRun(
                        PredefinedClassificationTypeNames.Identifier,
                        "and it is " + DateTime.Now.ToShortTimeString()));

            return Task.FromResult<object>(new ContainerElement(
                ContainerElementStyle.Stacked,
                content,
                lineInfo,
                timeInfo));
        }
    }
}