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
using OneCode.Shared;
using OneCode.Shared.Settings;
using System.Linq;

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
            var repositories = new Repositories();
            repositories.Load(OneCodeSettings.DefaultSettings);
            
            var items = repositories.Values
                .SelectMany(repository => repository.Files)
                .SelectMany(file => file.Code.Classes)
                .SelectMany(@class => @class.Methods)
                .Where(method => method.IsStatic)
                .Select(method =>
                {
                    var item = CSharpLookupItemFactory.Instance.CreateKeywordLookupItem(context, $"{method.Class?.Name}.{method.Name.Substring(0, method.Name.IndexOf('(') + 1)}", TailType.None, PsiSymbolsThemedIcons.Method.Id);
                
                    return item;
                })
                .ToList();

            foreach (var item in items)
            {
                item.SetInsertCaretOffset(-1);
                item.SetReplaceCaretOffset(-1);
                item.WithInitializedRanges(context.CompletionRanges, context.BasicContext);
                item.SetTopPriority();
                //item.Behavior = new WithReferenceBehaviorWrapper(new DeclaredElementWithReferenceInfo(), );\
                
                collector.Add(item);
            }

            return true;
        }
    }
}
