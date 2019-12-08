using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;

namespace OneCode.VsExtension.Utilities
{
    public static class ControlExtensions
    {
        public static bool? ShowAsDialog(this Control control, string title, double? width = null, double? height = null)
        {
            return new Window
            {
                Title = title,
                Content = control,
                Background = VsBrushes.WindowKey as Brush,
                Foreground = VsBrushes.WindowTextKey as Brush,
                Width = width ?? (double)FrameworkElement.WidthProperty.DefaultMetadata.DefaultValue,
                Height = height ?? (double)FrameworkElement.HeightProperty.DefaultMetadata.DefaultValue,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            }.ShowDialog();
        }

        public static bool? ShowAsDialog<T>(this object viewModel, string title, double? width = null, double? height = null) where T : Control, new()
        {
            return new T
            {
                DataContext = viewModel,
            }.ShowAsDialog(title, width, height);
        }
    }
}
