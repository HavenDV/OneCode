using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace OneCode.VsExtension
{
    /// <summary>
    /// Interaction logic for OneCodeWindowControl.
    /// </summary>
    public partial class OneCodeWindowControl
    {
        private Dictionary<string, Dictionary<string, string>> Methods { get; set; }

        public ObservableCollection<Node> Nodes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneCodeWindowControl"/> class.
        /// </summary>
        public OneCodeWindowControl()
        {
            InitializeComponent();
        }

        /*
        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(CultureInfo.CurrentUICulture, $"Invoked '{this}'"),
                "OneCodeWindow");
        }
        */

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string path;
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK ||
                    string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    return;
                }

                path = dialog.SelectedPath;
            }

            Methods = Core.Repository.Load(path);

            Nodes = new ObservableCollection<Node>(
                Methods.Select(pair => new Node
                {
                    Name = pair.Key,
                    Nodes = new ObservableCollection<Node>(pair.Value.Select(i => new Node
                    {
                        Name = i.Key,
                    }))
                }));

            TreeView.ItemsSource = Nodes;
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }
    }
}