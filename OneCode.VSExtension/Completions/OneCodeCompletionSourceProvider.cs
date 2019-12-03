using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace OneCode.VsExtension.Completions
{
    [Export(typeof(IAsyncCompletionSourceProvider))]
    [ContentType("CSharp")]
    [Name("Hello World completion item source")]
    internal class OneCodeCompletionSourceProvider : IAsyncCompletionSourceProvider
    {
        private Lazy<OneCodeCompletionSource> Source { get; } = new Lazy<OneCodeCompletionSource>();

        public IAsyncCompletionSource GetOrCreate(ITextView textView)
        {
            return Source.Value;
        }
    }
}
