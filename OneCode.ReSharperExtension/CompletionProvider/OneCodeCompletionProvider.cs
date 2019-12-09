using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Features.Intellisense.CodeCompletion.CSharp;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExpectedTypes;
using JetBrains.ReSharper.Psi.Resources;
using OneCode.ReSharperExtension.Extensions;

namespace OneCode.ReSharperExtension.CompletionProvider
{
    [Language(typeof(CSharpLanguage))]
    public class OneCodeCompletionProvider : ItemsProviderOfSpecificContext<CSharpCodeCompletionContext>
    {
        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            var codeCompletionType = context.BasicContext.CodeCompletionType;

            return codeCompletionType == CodeCompletionType.SmartCompletion || codeCompletionType == CodeCompletionType.BasicCompletion;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, IItemsCollector collector)
        {
            var proposedCallback = "ONE_CODE_5";
            var item = CSharpLookupItemFactory.Instance.CreateKeywordLookupItem(context, proposedCallback, TailType.None, PsiSymbolsThemedIcons.Method.Id);
            
            item.SetInsertCaretOffset(-1);
            item.SetReplaceCaretOffset(-1);
            item.WithInitializedRanges(context.CompletionRanges, context.BasicContext);
            item.SetTopPriority();
            collector.Add(item);

            return true;
        }
    }
}
