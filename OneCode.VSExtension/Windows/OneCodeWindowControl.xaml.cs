using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using OneCode.Core;
using OneCode.VsExtension.Properties;
using OneCode.VsExtension.Utilities;

namespace OneCode.VsExtension.Windows
{
    /// <summary>
    /// Interaction logic for OneCodeWindowControl.
    /// </summary>
    public partial class OneCodeWindowControl
    {
        public Repositories Repositories { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneCodeWindowControl"/> class.
        /// </summary>
        public OneCodeWindowControl()
        {
            InitializeComponent();

            Repositories = OneCodePackage.Repositories.GetValue();
            Repositories.Changed += (sender, args) => RefreshTree(Repositories);

            RefreshTree(Repositories);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK ||
                string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                return;
            }

            var path = dialog.SelectedPath;

            Repositories.Load(path);

            Settings.Default.RepositoryPath = path;
            Settings.Default.Save();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Repositories.Reload();
        }

        private void RefreshTree(Repositories repositories)
        {
            TreeView.ItemsSource = new ObservableCollection<Node>(new[]
            {
                new Node
                {
                    Name = "Static methods",
                    Nodes = new ObservableCollection<Node>(repositories.Values
                        .SelectMany(repository => repository.Files)
                        .Where(file => file.Code.Classes.Any(@class => @class.IsStatic && @class.Methods.Any(method => method.IsStatic)))
                        .Select(file => new Node
                        {
                            Name = file.RelativePath,
                            Nodes = new ObservableCollection<Node>(file.Code.Classes
                                .Where(@class => @class.IsStatic || @class.Methods.Any(method => method.IsStatic))
                                .Select(@class => @class.Methods.
                                    Select(method => new Node
                                    {
                                        Name = $"{@class.Name}.{method.Name}",
                                        Method = method,
                                        Class = @class,
                                        CodeFile = file,
                                    }))
                                .SelectMany(i => i)),
                        })),
                },
                new Node
                {
                    Name = "Classes",
                    Nodes = new ObservableCollection<Node>(repositories.Values
                        .SelectMany(repository => repository.Files)
                        .Where(file => file.Code.Classes.Any(@class => !@class.IsStatic))
                        .Select(file => new Node
                        {
                            Name = file.RelativePath,
                            Nodes = new ObservableCollection<Node>(file.Code.Classes
                                .Where(@class => !@class.IsStatic)
                                .Select(@class => new Node
                                {
                                    Name = @class.Name,
                                    Class = @class,
                                    CodeFile = file,
                                })),
                        })),
                },
            });
        }

        private void TreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = sender as TreeViewItem;
            var node = item?.Header as Node;

            OneCodePackage.AddItem(node?.CodeFile, node?.Class, node?.Method, true);
        }

        private void ShowRepositoriesButton_OnClick(object sender, RoutedEventArgs e)
        {
            OneCodePackage.Instance.ShowWindow(typeof(RepositoriesWindow));
        }
    }
}