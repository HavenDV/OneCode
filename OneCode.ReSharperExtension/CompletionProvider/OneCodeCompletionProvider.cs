using JetBrains.ProjectModel;
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
using System.Linq;
using OneCode.ReSharperExtension.Services;

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
            var repositoriesService = context.AccessContext.GetPsiModule().GetSolution().GetComponent<IRepositoriesService>();
            repositoriesService.LoadIfRequiredFromSettings();

            var items = repositoriesService.Repositories.AllMethods
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

            var firstMethod = repositoriesService.Repositories.AllMethods.First();
            repositoriesService.AddProjectItem(context.AccessContext.GetPsiModule().GetSolution().GetAllProjects().First(project => project.IsOpened), firstMethod.Class?.Code?.CodeFile, firstMethod.Class, firstMethod);

            return true;
        }
    }
}
