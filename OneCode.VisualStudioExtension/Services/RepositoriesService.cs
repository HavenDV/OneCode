using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using OneCode.Shared;
using OneCode.Shared.Settings;
using OneCode.VsExtension.Utilities;

namespace OneCode.VsExtension.Services
{
    [Export]
    public class RepositoriesService
    {
        public Repositories Repositories { get; set; } = new Repositories();

        public void LoadFromSettings()
        {
            Repositories.Load(OneCodeSettings.DefaultSettings);
        }

        public void AddAndSaveSettings(RepositorySettings settings)
        {
            Repositories.Add(settings);

            OneCodeSettings.DefaultSettings = Repositories.Settings;
        }

        public void RemoveAndSaveSettings(RepositorySettings settings)
        {
            Repositories.Remove(settings);

            OneCodeSettings.DefaultSettings = Repositories.Settings;
        }

        public void AddProjectItem(CodeFile file, Class @class, Method method, bool openAfterAdd = false)
        {
            if (file == null)
            {
                return;
            }

            var dte = this.GetDte();
            var project = dte.GetActiveProject();

            //var solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            //IVsHierarchy hierarchy;
            //solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);

            var projectDirectory = project.GetDirectory();
            var fullPath = Path.Combine(projectDirectory ?? string.Empty, file.RelativePathWithoutTargetFramework);

            file.Code.NamespaceName = project.GetDefaultNamespace() + file.AdditionalNamespace;
            file.Code.Classes = new List<Class> { @class };

            if (method != null)
            {
                file.Code.Classes[0].Methods = new List<Method> { method };
            }

            file.SaveTo(fullPath);

            project.AddItemFromFile(fullPath);

            if (openAfterAdd)
            {
                dte.OpenFileAsText(fullPath);
            }
        }
    }
}
