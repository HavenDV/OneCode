using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace OneCode.VsExtension.Utilities
{
    public static class AsyncPackageExtensions
    {
        public static void ShowWindow(this AsyncPackage package, Type type)
        {
            package.JoinableTaskFactory.RunAsync(async () =>
            {
                var window = await package.ShowToolWindowAsync(type, 0, true, package.DisposalToken);
                if (window?.Frame == null)
                {
                    throw new NotSupportedException("Cannot create tool window");
                }
                
                await package.JoinableTaskFactory.SwitchToMainThreadAsync();

                var frame = (IVsWindowFrame)window.Frame;
                ErrorHandler.ThrowOnFailure(frame.Show());
            });
        }
    }
}
