using JetBrains.ReSharper.Psi.Modules;

namespace OneCode.ReSharperExtension.Services
{
    public interface ITestProjectProvider
    {
        bool IsTestProject(IPsiModule psiModuleDisplayName);
    }
}
