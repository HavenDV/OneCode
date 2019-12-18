using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OneCode.VsExtension.UI.ViewModels;

#nullable enable

namespace OneCode.VsExtension.UI.Controls
{
    /// <summary>
    /// Interaction logic for OneCodeWindowControl.
    /// </summary>
    public sealed partial class OneCodeWindowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneCodeWindowControl"/> class.
        /// </summary>
        public OneCodeWindowControl()
        {
            InitializeComponent();
        }

        private void TreeView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TreeViewItem item) ||
                !item.IsSelected ||
                !(item.Header is Node node) ||
                !(DataContext is OneCodeViewModel viewModel))
            {
                return;
            }

            var command = viewModel.AddItemCommand;
            if (command.CanExecute(node))
            {
                command.Execute(node);
            }
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem == null)
            {
                return;
            }

            treeViewItem.Focus();
            e.Handled = true;
        }

        private static TreeViewItem? VisualUpwardSearch(DependencyObject? source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }
    }
}