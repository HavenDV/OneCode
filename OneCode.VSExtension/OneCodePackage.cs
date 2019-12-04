using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using OneCode.Core;
using OneCode.VsExtension.Commands;
using OneCode.VsExtension.Properties;
using OneCode.VsExtension.UI.Windows;
using OneCode.VsExtension.Utilities;
using Task = System.Threading.Tasks.Task;

namespace OneCode.VsExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(OneCodeWindow),
        Style = VsDockStyle.Tabbed,
        Window = EnvDTE.Constants.vsWindowKindSolutionExplorer)]
    public sealed class OneCodePackage : AsyncPackage
    {
        /// <summary>
        /// OneCodeExtensionPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "7ff6c859-ac79-49e7-98cf-70dfcf6a101d";
        
        #region OneCode

        public static AsyncLazy<Repositories> Repositories { get; set; } = new AsyncLazy<Repositories>(() => Task.Run(() =>
        {
            var repositories = new Repositories();

            repositories.Load(Settings.Default.RepositoryPath.Split(';').ToList());

            return repositories;
        }), ThreadHelper.JoinableTaskFactory);

        public static OneCodePackage Instance { get; set; }

        public static void AddItem(CodeFile file, Class @class, Method method, bool openAfterAdd = false)
        {
            if (file == null || Instance == null)
            {
                return;
            }

            var dte = Instance.GetDte();
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

        #endregion

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            progress.Report(new ServiceProgressData("Initializing windows..."));

            await OneCodeWindowCommand.InitializeAsync(this);
            
            Instance = this;
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            if (toolWindowType == typeof(OneCodeWindow).GUID)
            {
                return this;
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            return base.GetAsyncToolWindowFactory(toolWindowType);
        }

        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            if (toolWindowType == typeof(OneCodeWindow))
            {
                return "Loading OneCode Window...";
            }

            return base.GetToolWindowTitle(toolWindowType, id);
        }

        protected override async Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            if (toolWindowType == typeof(OneCodeWindow))
            {
                await Repositories.GetValueAsync(cancellationToken);
            }

            return null;
        }

        #endregion
    }
}
