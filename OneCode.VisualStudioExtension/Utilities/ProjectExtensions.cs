using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace OneCode.VsExtension.Utilities
{
    public static class ProjectExtensions
    {
        public static string GetDirectory(this Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return Path.GetDirectoryName(project.FileName);
        }

        public static string GetDefaultNamespace(this Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return project.Name;
        }

        public static void AddItemFromFile(this Project project, string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            project.ProjectItems.AddFromFile(path);
        }
    }
}
