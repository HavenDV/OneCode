using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using OneCode.VsExtension.Services;

#nullable enable

namespace OneCode.VsExtension.Completions
{
    [Export(typeof(IAsyncCompletionCommitManagerProvider))]
    [Name(nameof(OneCodeCompletionCommitManagerProvider))]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public sealed class OneCodeCompletionCommitManagerProvider : IAsyncCompletionCommitManagerProvider
    {
        private IDictionary<ITextView, IAsyncCompletionCommitManager> Cache { get; } = new Dictionary<ITextView, IAsyncCompletionCommitManager>();

        [Import]
        private RepositoriesService? RepositoriesService { get; set; }

        public IAsyncCompletionCommitManager GetOrCreate(ITextView textView)
        {
            if (Cache.TryGetValue(textView, out var itemSource))
            {
                return itemSource;
            }

            var manager = new OneCodeCompletionCommitManager(RepositoriesService);
            textView.Closed += (o, e) => Cache.Remove(textView); // clean up memory as files are closed
            Cache.Add(textView, manager);
            return manager;
        }
    }
}
