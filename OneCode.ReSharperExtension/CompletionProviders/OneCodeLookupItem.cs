using System;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.DocumentManagers.Transactions;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.Match;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Features.Intellisense.CodeCompletion.CSharp;
using JetBrains.TextControl;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.Util.Logging;
using OneCode.ReSharperExtension.Services;
using OneCode.Shared;

namespace OneCode.ReSharperExtension.CompletionProviders
{
    internal class OneCodeLookupItem : ILookupItem
    {
        private static ILogger Log { get; } = Logger.GetLogger(typeof(OneCodeLookupItem));

        private ILookupItem InternalItem { get; }
        private Method Method { get; }
        private CSharpCodeCompletionContext Context { get; }


        public bool IgnoreSoftOnSpace => InternalItem.IgnoreSoftOnSpace;

        public LookupItemPlacement Placement => InternalItem.Placement;

        public OneCodeLookupItem(Method method, CSharpCodeCompletionContext context)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Context = context ?? throw new ArgumentNullException(nameof(context));

            var shortcut = $"{method.Class?.Name}.{method.Name.Substring(0, method.Name.IndexOf('(') + 1)})";

            InternalItem = CSharpLookupItemFactory.Instance.CreateTextLookupItem(context.CompletionRanges, shortcut);
            //CreateKeywordLookupItem(context, shortcut, TailType.None, PsiSymbolsThemedIcons.Method.Id);

            InternalItem.SetInsertCaretOffset(-1);
            InternalItem.SetReplaceCaretOffset(-1);
            InternalItem.WithInitializedRanges(context.CompletionRanges, context.BasicContext);
            InternalItem.WithHighSelectionPriority();

            //Log.Info("Creating ZenSharpLookupItem with template = {0}", template);
        }

        public RichText DisplayName => InternalItem.DisplayName;
        public bool CanShrink => InternalItem.CanShrink;
        public bool IsDynamic => InternalItem.IsDynamic;

        public bool AcceptIfOnlyMatched(LookupItemAcceptanceContext itemAcceptanceContext)
        {
            return InternalItem.AcceptIfOnlyMatched(itemAcceptanceContext);
        }

        public void Accept(
            ITextControl textControl,
            DocumentRange nameRange,
            LookupItemInsertType insertType,
            Suffix suffix,
            ISolution solution,
            bool keepCaretStill)
        {
            InternalItem.Accept(textControl, nameRange, insertType, suffix, solution, keepCaretStill);

            var project = solution
                .GetAllProjects()
                .FirstOrDefault(i => i
                    .GetAllProjectFiles(j => j.Location.FullPath == textControl.Document.Moniker)
                    .Any());
            if (project == null)
            {
                throw new InvalidOperationException("Project of selection file is not found");
            }

            var folder = project
                .GetSubFolders()
                .FirstOrDefault(i => i.Location == FileSystemPath.CreateByCanonicalPath(textControl.Document.Moniker).Directory);
            if (folder == null)
            {
                throw new InvalidOperationException("Folder of selection file is not found");
            }

            var repositoriesService = solution.GetComponent<IRepositoriesService>();
            var path = repositoriesService.AddProjectItem(project, Method.Class?.Code?.CodeFile, Method.Class, Method);
            var fileSystemPath = FileSystemPath.CreateByCanonicalPath(path);

            {
                using var cookie = solution.CreateTransactionCookie(DefaultAction.Commit, "Create file", NullProgressIndicator.Create());
                if (!cookie.CanAddFile(folder, fileSystemPath, out var reason))
                {
                    throw new InvalidOperationException($"Can't add file: {reason}");
                }

                cookie.AddFile(folder, fileSystemPath);
            }

            var usingText = $"using {Method.Class?.Code?.NamespaceName};{Environment.NewLine}";
            textControl.Document.InsertText(0, usingText);
        }

        public MatchingResult Match(PrefixMatcher prefixMatcher)
        {
            return InternalItem.Match(prefixMatcher);
        }

        public bool Shrink()
        {
            return InternalItem.Shrink();
        }

        public void Unshrink()
        {
            InternalItem.Unshrink();
        }

        public int Identity => InternalItem.Identity;

        public IconId Image => InternalItem.Image;

        public RichText DisplayTypeName => InternalItem.DisplayTypeName;

        public DocumentRange GetVisualReplaceRange(DocumentRange nameRange)
        {
            return InternalItem.GetVisualReplaceRange(nameRange);
        }
    }
}
