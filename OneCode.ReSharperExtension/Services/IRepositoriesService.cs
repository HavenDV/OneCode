using JetBrains.ProjectModel;
using OneCode.Shared;

namespace OneCode.ReSharperExtension.Services
{
    public interface IRepositoriesService
    {
        Repositories Repositories { get; }

        void LoadIfRequiredFromSettings();
        void AddProjectItem(IProject project, CodeFile? file, Class? @class, Method? method);
    }
}
