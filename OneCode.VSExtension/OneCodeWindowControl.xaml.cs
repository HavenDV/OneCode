using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using OneCode.Core;
using OneCode.VsExtension.Properties;
using OneCode.VsExtension.Utilities;

namespace OneCode.VsExtension
{
    /// <summary>
    /// Interaction logic for OneCodeWindowControl.
    /// </summary>
    public partial class OneCodeWindowControl
    {
        private Repository Repository { get; set; }

        public ObservableCollection<Node> Nodes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneCodeWindowControl"/> class.
        /// </summary>
        public OneCodeWindowControl()
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(Settings.Default.RepositoryPath))
            {
                Load(Settings.Default.RepositoryPath);
            }
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
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK ||
                    string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    return;
                }

                Load(dialog.SelectedPath);
            }
        }

        private void Load(string path)
        {
            Settings.Default.RepositoryPath = path;
            Settings.Default.Save();

            Repository = Repository.Load(path);

            Nodes = new ObservableCollection<Node>(
                Repository.Files.Select(file => new Node
                {
                    Name = file.RelativePath,
                    Nodes = new ObservableCollection<Node>(file.Code.Methods.Select(method => new Node
                    {
                        Name = method.Name,
                        Method = method,
                        CodeFile = file,
                    }))
                }));

            TreeView.ItemsSource = Nodes;
        }


        private void AddItem(Node node)
        {
            if (node?.CodeFile == null)
            {
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            var project = dte.GetActiveProject();

            //var solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            //IVsHierarchy hierarchy;
            //solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);

            var prefix = node.CodeFile.RelativePath.TrimStart('\\', '/');
            var index = prefix.IndexOfAny(new []{ '\\', '/' });
            var relativePath = index > 0
                ? prefix.Substring(index + 1)
                : prefix;

            var projectDirectory = Path.GetDirectoryName(project.FileName);
            var fullPath = Path.Combine(projectDirectory ?? string.Empty, relativePath);
            var directory = Path.GetDirectoryName(fullPath);

            node.CodeFile.Code.NamespaceName = project.Name;
            node.CodeFile.Code.Methods = new List<Method> { node.Method };

            if (!File.Exists(fullPath))
            {
                if (directory != null)
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(fullPath, node.CodeFile.Code.Save());
            }
            else
            {
                CodeFile
                    .Load(fullPath)
                    .Merge(node.CodeFile)
                    .Save();
            }

            project.ProjectItems.AddFromFile(fullPath);

            dte.ItemOperations.OpenFile(fullPath, Constants.vsViewKindTextView);
        }

        private void TreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = sender as TreeViewItem;
            var node = item?.Header as Node;

            ThreadHelper.ThrowIfNotOnUIThread();

            AddItem(node);
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public Method Method { get; set; }
        public CodeFile CodeFile { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }
    }
}