using System.Collections.Generic;
using System.IO;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;
using OneCode.Shared;
using OneCode.Shared.Settings;

namespace OneCode.ReSharperExtension.Services
{
    [SolutionComponent]
    public class RepositoriesService : IRepositoriesService
    {
        public Repositories Repositories { get; } = new Repositories();

        public void LoadIfRequiredFromSettings()
        {
            if (Repositories.IsLoaded)
            {
                return;
            }

            Repositories.Load(OneCodeSettings.DefaultSettings);
        }

        public string? AddProjectItem(IProject project, CodeFile? file, Class? @class, Method? method)
        {
            if (file == null || project == null)
            {
                return null;
            }

            var projectDirectory = project.GetLocation().FullPath;
            var fullPath = Path.Combine(projectDirectory, file.RelativePathWithoutTargetFramework);

            if (file.Code != null)
            {
                file.Code.NamespaceName = project.GetDefaultNamespace() + file.AdditionalNamespace;
                file.Code.Classes = @class != null ? new List<Class> { @class } : file.Code.Classes;

                if (method != null)
                {
                    file.Code.Classes[0].Methods = new List<Method> { method };
                }
            }

            file.SaveTo(fullPath);

            return fullPath;
        }
    }
}