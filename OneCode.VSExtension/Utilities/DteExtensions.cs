using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace OneCode.VsExtension.Utilities
{
    public static class DteExtensions
    {
        public static Project GetActiveProject(this DTE dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return (dte.ActiveSolutionProjects as Array)?
                .Cast<Project>()
                .FirstOrDefault();
        }

        public static DTE GetDte(this object _)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return (DTE)Package.GetGlobalService(typeof(DTE));
        }

        public static Window OpenFileAsText(this DTE dte, string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return dte.ItemOperations.OpenFile(path, Constants.vsViewKindTextView);
        }
    }
}
