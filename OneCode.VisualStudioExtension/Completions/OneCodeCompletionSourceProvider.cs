using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using OneCode.VsExtension.Services;

#nullable enable

namespace OneCode.VsExtension.Completions
{
    [Export(typeof(IAsyncCompletionSourceProvider))]
    [Name(nameof(OneCodeCompletionSourceProvider))]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Order(After = "default")]
    [Order(After = "Roslyn Completion Presenter")]
    public sealed class OneCodeCompletionSourceProvider : IAsyncCompletionSourceProvider
    {
        private IDictionary<ITextView, IAsyncCompletionSource> CacheDictionary { get; } = new Dictionary<ITextView, IAsyncCompletionSource>();

        [Import]
        private RepositoriesService? RepositoriesService { get; set; }

        public IAsyncCompletionSource GetOrCreate(ITextView textView)
        {
            if (CacheDictionary.TryGetValue(textView, out var cachedSource))
            {
                return cachedSource;
            }

            var source = new OneCodeCompletionSource(RepositoriesService);
            textView.Closed += (o, e) => CacheDictionary.Remove(textView);
            CacheDictionary.Add(textView, source);

            return source;
        }
    }
}
