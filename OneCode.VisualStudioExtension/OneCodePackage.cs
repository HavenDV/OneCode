﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OneCode.VsExtension.Commands;
using OneCode.VsExtension.Services;
using OneCode.VsExtension.UI.Windows;
using Task = System.Threading.Tasks.Task;

#nullable enable

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

        #region Properties

        public RepositoriesService? RepositoriesService { get; set; }
        public ExceptionsService? ExceptionsService { get; set; }

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
            progress.Report(new ServiceProgressData("Initializing services..."));

            var componentModel = await GetServiceAsync(typeof(SComponentModel)) as IComponentModel ?? throw new InvalidOperationException("Service SComponentModel is not found");
            RepositoriesService = componentModel.DefaultExportProvider.GetExportedValue<RepositoriesService>();
            ExceptionsService = componentModel.DefaultExportProvider.GetExportedValue<ExceptionsService>();

            progress.Report(new ServiceProgressData("Loading repositories..."));

            await JoinableTaskFactory.RunAsync(() => Task.Run(() =>
            {
                try
                {
                    RepositoriesService.LoadFromSettings();
                }
                catch (Exception exception)
                {
                    ExceptionsService.Add(exception);
                }
            }, cancellationToken));

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            progress.Report(new ServiceProgressData("Initializing windows..."));

            await OneCodeWindowCommand.InitializeAsync(this);
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

        protected override Task<object?> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            if (toolWindowType == typeof(OneCodeWindow))
            {
                return Task.FromResult<object?>(this);
            }

            return Task.FromResult<object?>(null);
        }

        #endregion
    }
}
