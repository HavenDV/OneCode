using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace OneCode.VsExtension.Utilities
{
    public static class DteExtensions
    {
        public static Project GetActiveProject(this DTE source)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return (source.ActiveSolutionProjects as Array)?
                .Cast<Project>()
                .FirstOrDefault();
        }
    }
}
