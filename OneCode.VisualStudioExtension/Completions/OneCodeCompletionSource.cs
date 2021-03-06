﻿using System;
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
using OneCode.Shared;
using OneCode.VsExtension.Services;
using OneCode.VsExtension.Utilities;
using Task = System.Threading.Tasks.Task;

#nullable enable

namespace OneCode.VsExtension.Completions
{
    public sealed class OneCodeCompletionSource : IAsyncCompletionSource
    {
        private static ImageElement ImageElement { get; } = new ImageElement(new ImageId(KnownImageIds.ImageCatalogGuid, KnownImageIds.AddMethod));
        private ImmutableArray<CompletionItem>? Items { get; set; }
        private ImmutableArray<CompletionFilter>? Filters { get; set; }
        private ImmutableArray<ImageElement>? Images { get; set; }
        private RepositoriesService RepositoriesService { get; }

        public OneCodeCompletionSource(RepositoriesService? repositoriesService)
        {
            RepositoriesService = repositoriesService ?? throw new ArgumentNullException(nameof(repositoriesService));
            RepositoriesService.Repositories.Changed += (sender, args) => Items = GetActualItems().ToImmutableArray();
        }

        public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token)
        {
            // Since we are plugging in to CSharp content type,
            // allow the CSharp language service to pick the Applicable To Span.
            return CompletionStartData.ParticipatesInCompletionIfAny;
            // Alternatively, we've got to provide location for completion
            // return new CompletionStartData(CompletionParticipation.ProvidesItems, ...

            /*
            // We don't trigger completion when user typed
            if (char.IsNumber(trigger.Character)         // a number
                || char.IsPunctuation(trigger.Character) // punctuation
                || trigger.Character == '\n'             // new line
                || trigger.Reason == CompletionTriggerReason.Backspace
                || trigger.Reason == CompletionTriggerReason.Deletion)
            {
                return CompletionStartData.DoesNotParticipateInCompletion;
            }

            // We participate in completion and provide the "applicable to span".
            // This span is used:
            // 1. To search (filter) the list of all completion items
            // 2. To highlight (bold) the matching part of the completion items
            // 3. In standard cases, it is replaced by content of completion item upon commit.

            // If you want to extend a language which already has completion, don't provide a span, e.g.
            // return CompletionStartData.ParticipatesInCompletionIfAny

            // If you provide a language, but don't have any items available at this location,
            // consider providing a span for extenders who can't parse the codem e.g.
            // return CompletionStartData(CompletionParticipation.DoesNotProvideItems, spanForOtherExtensions);

            var tokenSpan = FindTokenSpanAtPosition(triggerLocation);
            return new CompletionStartData(CompletionParticipation.ProvidesItems, tokenSpan);
            */
        }

        /*
        private SnapshotSpan FindTokenSpanAtPosition(SnapshotPoint triggerLocation)
        {
            // This method is not really related to completion,
            // we mostly work with the default implementation of ITextStructureNavigator 
            // You will likely use the parser of your language
            ITextStructureNavigator navigator = StructureNavigatorSelector.GetTextStructureNavigator(triggerLocation.Snapshot.TextBuffer);
            TextExtent extent = navigator.GetExtentOfWord(triggerLocation);
            if (triggerLocation.Position > 0 && (!extent.IsSignificant || !extent.Span.GetText().Any(c => char.IsLetterOrDigit(c))))
            {
                // Improves span detection over the default ITextStructureNavigation result
                extent = navigator.GetExtentOfWord(triggerLocation - 1);
            }

            var tokenSpan = triggerLocation.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);

            var snapshot = triggerLocation.Snapshot;
            var tokenText = tokenSpan.GetText(snapshot);
            if (string.IsNullOrWhiteSpace(tokenText))
            {
                // The token at this location is empty. Return an empty span, which will grow as user types.
                return new SnapshotSpan(triggerLocation, 0);
            }

            // Trim quotes and new line characters.
            int startOffset = 0;
            int endOffset = 0;

            if (tokenText.Length > 0)
            {
                if (tokenText.StartsWith("\""))
                    startOffset = 1;
            }
            if (tokenText.Length - startOffset > 0)
            {
                if (tokenText.EndsWith("\"\r\n"))
                    endOffset = 3;
                else if (tokenText.EndsWith("\r\n"))
                    endOffset = 2;
                else if (tokenText.EndsWith("\"\n"))
                    endOffset = 2;
                else if (tokenText.EndsWith("\n"))
                    endOffset = 1;
                else if (tokenText.EndsWith("\""))
                    endOffset = 1;
            }

            return new SnapshotSpan(tokenSpan.GetStartPoint(snapshot) + startOffset, tokenSpan.GetEndPoint(snapshot) - endOffset);
        }
        */

        public IEnumerable<CompletionItem> GetActualItems()
        {
            return RepositoriesService.Repositories.AllMethods
                .Where(method => method.IsStatic)
                .Select(method =>
                {
                    var item = new CompletionItem(
                        $"{method.Class?.Name}.{method.Name}",
                        this,
                        ImageElement,
                        Filters ?? ImmutableArray<CompletionFilter>.Empty,
                        method.Class?.Code?.NamespaceName,
                        $"{method.Class?.Name}.{method.Name.Substring(0, method.Name.IndexOf('(') + 1)}",
                        $"{method.Class?.Name}.{method.Name}",
                        $"{method.Class?.Name}.{method.Name}",
                        Images ?? ImmutableArray<ImageElement>.Empty);

                    item.Properties[nameof(Method)] = method;
                    item.Properties[nameof(Class)] = method.Class;
                    item.Properties[nameof(Code)] = method.Class?.Code;
                    item.Properties[nameof(CodeFile)] = method.Class?.Code?.CodeFile;
                    item.Properties[nameof(Repository)] = method.Class?.Code?.CodeFile?.Repository;

                    return item;
                });
        }

        public Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
        {
            Filters ??= ImmutableArray.Create(new CompletionFilter("OneCode", "O", ImageElement));
            Images ??= ImmutableArray.Create<ImageElement>();
            Items ??= GetActualItems().ToImmutableArray();

            return Task.FromResult(new CompletionContext(Items ?? ImmutableArray<CompletionItem>.Empty));

            /*
            // See whether we are in the key or value portion of the pair
            var lineStart = triggerLocation.GetContainingLine().Start;
            var spanBeforeCaret = new SnapshotSpan(lineStart, triggerLocation);
            var textBeforeCaret = triggerLocation.Snapshot.GetText(spanBeforeCaret);
            var colonIndex = textBeforeCaret.IndexOf(':');
            var colonExistsBeforeCaret = colonIndex != -1;

            // User is likely in the key portion of the pair
            if (!colonExistsBeforeCaret)
                return GetContextForKey();

            // User is likely in the value portion of the pair. Try to provide extra items based on the key.
            var KeyExtractingRegex = new Regex(@"\W*(\w+)\W*:");
            var key = KeyExtractingRegex.Match(textBeforeCaret);
            var candidateName = key.Success ? key.Groups.Count > 0 && key.Groups[1].Success ? key.Groups[1].Value : string.Empty : string.Empty;
            return GetContextForValue(candidateName);*/
        }
        /*
        /// <summary>
        /// Returns completion items applicable to the value portion of the key-value pair
        /// </summary>
        private CompletionContext GetContextForValue(string key)
        {
            // Provide a few items based on the key
            ImmutableArray<CompletionItem> itemsBasedOnKey = ImmutableArray<CompletionItem>.Empty;
            if (!string.IsNullOrEmpty(key))
            {
                var matchingElement = Catalog.Elements.FirstOrDefault(n => n.Name == key);
                if (matchingElement != null)
                {
                    var itemsBuilder = ImmutableArray.CreateBuilder<CompletionItem>();
                    itemsBuilder.Add(new CompletionItem(matchingElement.Name, this));
                    itemsBuilder.Add(new CompletionItem(matchingElement.Symbol, this));
                    itemsBuilder.Add(new CompletionItem(matchingElement.AtomicNumber.ToString(), this));
                    itemsBuilder.Add(new CompletionItem(matchingElement.AtomicWeight.ToString(), this));
                    itemsBasedOnKey = itemsBuilder.ToImmutable();
                }
            }
            // We would like to allow user to type anything, so we create SuggestionItemOptions
            var suggestionOptions = new SuggestionItemOptions("Value of your choice", $"Please enter value for key {key}");

            return new CompletionContext(itemsBasedOnKey, suggestionOptions);
        }

        /// <summary>
        /// Returns completion items applicable to the key portion of the key-value pair
        /// </summary>
        private CompletionContext GetContextForKey()
        {
            var context = new CompletionContext(Catalog.Elements.Select(n => MakeItemFromElement(n)).ToImmutableArray());
            return context;
        }*/

        public Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token)
        {
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