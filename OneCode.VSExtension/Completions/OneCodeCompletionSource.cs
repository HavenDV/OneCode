using System.Collections.Generic;
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
using OneCode.VsExtension.Utilities;
using Task = System.Threading.Tasks.Task;

namespace OneCode.VsExtension.Completions
{
    public class OneCodeCompletionSource : IAsyncCompletionSource
    {
        private static ImageElement ImageElement { get; } = new ImageElement(new ImageId(KnownImageIds.ImageCatalogGuid, KnownImageIds.AddMethod));
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

        public IEnumerable<CompletionItem> GetActualItems()
        {
            return Repositories.Values
                .Select(repository => repository.Files
                    .Select(file => file.Code.Classes.Select(@class => @class.Methods
                            .Where(method => method.IsStatic)
                            .Select(method =>
                            {
                                var item = new CompletionItem(
                                    $"{@class.Name}.{method.Name}",
                                    this,
                                    ImageElement,
                                    Filters ?? ImmutableArray<CompletionFilter>.Empty,
                                    file.Code.NamespaceName,
                                    $"{@class.Name}.{method.Name.Substring(0, method.Name.IndexOf('(') + 1)}",
                                    $"{@class.Name}.{method.Name}",
                                    $"{@class.Name}.{method.Name}",
                                    Images ?? ImmutableArray<ImageElement>.Empty);

                                item.Properties[nameof(Repository)] = repository;
                                item.Properties[nameof(CodeFile)] = file;
                                item.Properties[nameof(Class)] = @class;
                                item.Properties[nameof(Method)] = method;

                                return item;
                            }))
                        .SelectMany(i => i))
                    .SelectMany(i => i))
                .SelectMany(i => i);
        }

        public async Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
        {
            if (Repositories == null)
            {
                Repositories ??= await OneCodePackage.Repositories.GetValueAsync(token);
                Repositories.Changed += (sender, args) => Items = GetActualItems().ToImmutableArray();
            }
            
            Filters ??= ImmutableArray.Create(new CompletionFilter("OneCode", "O", ImageElement));
            Images ??= ImmutableArray.Create<ImageElement>();
            Items ??= GetActualItems()?.ToImmutableArray();

            return new CompletionContext(Items ?? ImmutableArray<CompletionItem>.Empty);
        }

        public Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token)
        {
            /*
            var content = new ContainerElement(
                ContainerElementStyle.Wrapped,
                CompletionItemIcon,
                new ClassifiedTextElement(
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.Keyword, "Hello!"),
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.Identifier, " This is a sample item")));
            */
            var fileInfo = new ClassifiedTextElement(
                new ClassifiedTextRun(
                    PredefinedClassificationTypeNames.String,
                    "File: " + item.Properties.GetOrDefault<CodeFile>(nameof(CodeFile))?.RelativePath));
            var versionInfo = new ClassifiedTextElement(
                new ClassifiedTextRun(
                    PredefinedClassificationTypeNames.Comment,
                    "Version: " + item.Properties.GetOrDefault<Method>(nameof(Method))?.Version));
            var dependencyInfo = new ContainerElement(
                ContainerElementStyle.Stacked,
                new ClassifiedTextElement(item.Properties.GetOrDefault<Method>(nameof(Method))?.Dependencies
                    .Select(dependency => new ClassifiedTextRun(PredefinedClassificationTypeNames.Keyword, $"Dependency: {dependency}"))));
            var methodInfo = new ClassifiedTextElement(
                    new ClassifiedTextRun(
                        PredefinedClassificationTypeNames.Identifier,
                        "Method: " + item.Properties.GetOrDefault<Method>(nameof(Method))?.FullText));

            return Task.FromResult<object>(new ContainerElement(
                ContainerElementStyle.Stacked,
                fileInfo,
                versionInfo,
                dependencyInfo,
                methodInfo));
        }
    }
}