using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace OneCode.VsExtension.Completions
{
    [Export(typeof(IAsyncCompletionCommitManagerProvider))]
    [Name("OneCode commit manager provider")]
    [ContentType("CSharp")]
    internal class OneCodeCompletionCommitManagerProvider : IAsyncCompletionCommitManagerProvider
    {
        private IDictionary<ITextView, IAsyncCompletionCommitManager> Cache { get; } = new Dictionary<ITextView, IAsyncCompletionCommitManager>();

        public IAsyncCompletionCommitManager GetOrCreate(ITextView textView)
        {
            if (Cache.TryGetValue(textView, out var itemSource))
                return itemSource;

            var manager = new OneCodeCompletionCommitManager();
            textView.Closed += (o, e) => Cache.Remove(textView); // clean up memory as files are closed
            Cache.Add(textView, manager);
            return manager;
        }
    }
}
