using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;

namespace OneCode.ReSharperExtension.Extensions
{
    public class ComponentResolver
    {
        public static T GetComponent<T>(ICSharpContextActionDataProvider dataProvider ) where T : class => dataProvider.PsiModule.GetSolution().GetComponent<T>();
    }
}
