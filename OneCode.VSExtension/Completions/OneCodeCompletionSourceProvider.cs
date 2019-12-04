using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace OneCode.VsExtension.Completions
{
    [Export(typeof(IAsyncCompletionSourceProvider))]
    [Name(nameof(OneCodeCompletionSourceProvider))]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Order(After = "default")]
    [Order(After = "Roslyn Completion Presenter")]
    internal class OneCodeCompletionSourceProvider : IAsyncCompletionSourceProvider
    {
        private IDictionary<ITextView, IAsyncCompletionSource> CacheDictionary { get; } = new Dictionary<ITextView, IAsyncCompletionSource>();

        [Import]
        // ReSharper disable once InconsistentNaming
        private readonly ITextStructureNavigatorSelectorService StructureNavigatorSelector;

        public IAsyncCompletionSource GetOrCreate(ITextView textView)
        {
            if (CacheDictionary.TryGetValue(textView, out var cachedSource))
            {
                return cachedSource;
            }

            var source = new OneCodeCompletionSource(StructureNavigatorSelector);
            textView.Closed += (o, e) => CacheDictionary.Remove(textView);
            CacheDictionary.Add(textView, source);

            return source;
        }
    }
}
