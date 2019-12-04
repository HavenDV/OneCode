using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;

namespace OneCode.VsExtension.Utilities
{
    public static class ControlExtensions
    {
        public static bool? ShowAsDialog(this Control control, string title)
        {
            return new Window
            {
                Title = title,
                Content = control,
                Background = VsBrushes.WindowKey as Brush,
                Foreground = VsBrushes.WindowTextKey as Brush,
            }.ShowDialog();
        }
    }
}
