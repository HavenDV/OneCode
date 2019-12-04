using System.Windows.Controls;
using System.Windows.Input;
using OneCode.VsExtension.UI.ViewModels;

namespace OneCode.VsExtension.UI.Controls
{
    /// <summary>
    /// Interaction logic for OneCodeWindowControl.
    /// </summary>
    public partial class OneCodeWindowControl
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
    }
}