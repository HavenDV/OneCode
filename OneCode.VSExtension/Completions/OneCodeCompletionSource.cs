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

namespace OneCode.VsExtension.Completions
{
    public class OneCodeCompletionSource : IAsyncCompletionSource
    {
        private static ImageElement CompletionItemIcon { get; } = new ImageElement(new ImageId(new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369"), KnownImageIds.AddMethod), "Add Method Icon");
        private ImmutableArray<CompletionItem> Items { get; }
        private ImmutableArray<CompletionFilter> Filters { get; }
        private ImmutableArray<ImageElement> Images { get; }

        public OneCodeCompletionSource()
        {
            Filters = ImmutableArray.Create<CompletionFilter>();
            Images = ImmutableArray.Create<ImageElement>();
            Items = ImmutableArray.Create(OneCodePackage.Repositories.Values
                .SelectMany(repository => repository.Files)
                .Select(file => file.Code.Classes.Select(@class => @class.Methods
                        .Where(method => method.IsStatic)
                        .Select(method => new CompletionItem($"{@class.Name}.{method.Name}", this, CompletionItemIcon, Filters, file.Code.NamespaceName, $"{@class.Name}.{method.Name.Substring(0, method.Name.IndexOf('(') + 1)}", $"{@class.Name}.{method.Name}", $"{@class.Name}.{method.Name}", Images)))
                    .SelectMany(i => i))
                .SelectMany(i => i)
                .ToArray());
        }

        public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token)
        {
            // Since we are plugging in to CSharp content type,
            // allow the CSharp language service to pick the Applicable To Span.
            return CompletionStartData.ParticipatesInCompletionIfAny;
            // Alternatively, we've got to provide location for completion
            // return new CompletionStartData(CompletionParticipation.ProvidesItems, ...
        }

        public Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
        {
            session.Properties["LineNumber"] = triggerLocation.GetContainingLine().LineNumber;

            return Task.FromResult(new CompletionContext(Items));
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