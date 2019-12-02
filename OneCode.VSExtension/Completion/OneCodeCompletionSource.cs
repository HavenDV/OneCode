using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using OneCode.Core;
using OneCode.VsExtension.Properties;

namespace OneCode.VsExtension.Completion
{
    internal class OneCodeCompletionSource : ICompletionSource
    {
        private OneCodeCompletionSourceProvider SourceProvider { get; }
        private ITextBuffer TextBuffer { get; }
        private List<Microsoft.VisualStudio.Language.Intellisense.Completion> Completions { get; set; }
        private bool IsDisposed { get; set; }

        public OneCodeCompletionSource(OneCodeCompletionSourceProvider sourceProvider, ITextBuffer textBuffer)
        {
            SourceProvider = sourceProvider;
            TextBuffer = textBuffer;
        }

        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            var repository = Repository.Load(Settings.Default.RepositoryPath);
            var values = repository.Files
                .SelectMany(i => i.Code.Classes)
                .SelectMany(i => i.Methods)
                .Where(i => i.IsStatic)
                .Select(i => i.Name)
                .ToList();

            Completions = values.Select(value => new Microsoft.VisualStudio.Language.Intellisense.Completion(value, value, value, null, null)).ToList();

            completionSets.Add(new CompletionSet(
                "Tokens",    //the non-localized title of the tab
                "Tokens",    //the display title of the tab
                FindTokenSpanAtPosition(session.GetTriggerPoint(TextBuffer), session),
                Completions,
                null));
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        // ReSharper disable once UnusedParameter.Local
        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint _, ICompletionSession session)
        {
            var currentPoint = session.TextView.Caret.Position.BufferPosition - 1;
            var navigator = SourceProvider.NavigatorService.GetTextStructureNavigator(TextBuffer);
            var extent = navigator.GetExtentOfWord(currentPoint);

            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            GC.SuppressFinalize(this);
            IsDisposed = true;
        }
    }
}
