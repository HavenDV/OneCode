using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using System.Linq;
using OneCode.ReSharperExtension.Services;

namespace OneCode.ReSharperExtension.CompletionProviders
{
    [Language(typeof(CSharpLanguage))]
    public class OneCodeCompletionProvider : ItemsProviderOfSpecificContext<CSharpCodeCompletionContext>
    {
        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            var codeCompletionType = context.BasicContext.CodeCompletionType;

            return codeCompletionType == CodeCompletionType.SmartCompletion || 
                   codeCompletionType == CodeCompletionType.BasicCompletion;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, IItemsCollector collector)
        {
            var solution = context.BasicContext.Solution;
            //var iconManager = solution.GetComponent<PsiIconManager>();
            var repositoriesService = solution.GetComponent<IRepositoriesService>();
            //var iconId = iconManager.ExtendToTypicalSize(ServicesThemedIcons.LiveTemplate.Id);

            repositoriesService.LoadIfRequiredFromSettings();

            var methods = repositoriesService.Repositories.AllMethods
                .Where(method => method.IsStatic)
                .ToList();

            foreach (var method in methods)
            {
                collector.Add(new OneCodeLookupItem(method, context));
            }

            return true;
        }
    }
}
