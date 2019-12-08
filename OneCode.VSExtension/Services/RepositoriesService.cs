using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using OneCode.Core;
using OneCode.VsExtension.Properties;
using OneCode.VsExtension.Utilities;

namespace OneCode.VsExtension.Services
{
    [Export]
    public class RepositoriesService
    {
        public Repositories Repositories { get; set; } = new Repositories();

        public void LoadFromSettings()
        {
            var paths = Settings.Default.RepositoryPath?
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList() ?? new List<string>();

            Repositories.Load(paths);
        }

        public void AddAndSaveSettings(string path)
        {
            Repositories.Load(path);

            SaveSettings();
        }

        public void RemoveAndSaveSettings(Repository repository)
        {
            Repositories.Remove(repository);

            SaveSettings();
        }

        public void SaveSettings()
        {
            Settings.Default.RepositoryPath = string.Join(";", Repositories.Values.Select(i => i.Folder));
            Settings.Default.Save();
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
