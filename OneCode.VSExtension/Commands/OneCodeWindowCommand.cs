using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using OneCode.VsExtension.UI.Windows;
using OneCode.VsExtension.Utilities;
using Task = System.Threading.Tasks.Task;

namespace OneCode.VsExtension.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class OneCodeWindowCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("2695169c-5ac2-4680-a9d8-6140e6d5813b");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private AsyncPackage Package { get; }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static OneCodeWindowCommand Instance {
            get;
            private set;
        }

        /*
        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider => Package;
        */

        /// <summary>
        /// Initializes a new instance of the <see cref="OneCodeWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private OneCodeWindowCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandId);

            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in OneCodeWindowCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new OneCodeWindowCommand(package, commandService);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            Package.ShowWindow(typeof(OneCodeWindow));
        }
    }
}
