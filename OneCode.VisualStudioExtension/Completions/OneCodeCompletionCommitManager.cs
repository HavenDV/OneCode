using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using OneCode.Shared;
using OneCode.VsExtension.Services;
using OneCode.VsExtension.Utilities;

#nullable enable

namespace OneCode.VsExtension.Completions
{
    /// <summary>
    /// The simplest implementation of IAsyncCompletionCommitManager that provides Commit Characters and uses default behavior otherwise
    /// </summary>
    public sealed class OneCodeCompletionCommitManager : IAsyncCompletionCommitManager
    {
        private ImmutableArray<char> CommitChars { get; } = new [] { ' ', '\'', '"', ',', '.', ';', ':' }.ToImmutableArray();

        public IEnumerable<char> PotentialCommitCharacters => CommitChars;
        private RepositoriesService RepositoriesService { get; }

        public OneCodeCompletionCommitManager(RepositoriesService? repositoriesService)
        {
            RepositoriesService = repositoriesService ?? throw new ArgumentNullException(nameof(repositoriesService));
        }

        public bool ShouldCommitCompletion(IAsyncCompletionSession session, SnapshotPoint location, char typedChar, CancellationToken token)
        {
            // This method runs synchronously, potentially before CompletionItem has been computed.
            // The purpose of this method is to filter out characters not applicable at given location.

            // This method is called only when typedChar is among the PotentialCommitCharacters
            // in this simple example, all PotentialCommitCharacters do commit, so we always return true

            // Need testing
            //return true;

            return false;
        }

        public CommitResult TryCommit(IAsyncCompletionSession session, ITextBuffer buffer, CompletionItem item, char typedChar, CancellationToken token)
        {
            // Objects of interest here are session.TextView and session.TextView.Caret.
            // This method runs synchronously

            var file = item.Properties.GetOrDefault<CodeFile>(nameof(CodeFile));
            var @class = item.Properties.GetOrDefault<Class>(nameof(Class));
            var method = item.Properties.GetOrDefault<Method>(nameof(Method));
            
            if (!session.IsDismissed &&
                file != null)
            {
                RepositoriesService.AddProjectItem(file, @class, method);

                var usingText = $"using {file.Code.NamespaceName};{Environment.NewLine}";
                if (!buffer.CurrentSnapshot.GetText().Contains(usingText))
                {
                    buffer.Replace(Span.FromBounds(0, 1), usingText + "u");
                }
            }
            
            return CommitResult.Unhandled; // use default commit mechanism.
        }
    }
}