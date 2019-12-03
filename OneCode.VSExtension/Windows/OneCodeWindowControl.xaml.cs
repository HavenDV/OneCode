using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using OneCode.Core;
using OneCode.VsExtension.Utilities;

namespace OneCode.VsExtension.Windows
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

            OneCodePackage.Repositories.Changed += (sender, args) => RefreshTree(OneCodePackage.Repositories);

            RefreshTree(OneCodePackage.Repositories);
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

            OneCodePackage.Repositories.Load(path);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            OneCodePackage.Repositories.Reload();
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

        private void AddItem(Node node)
        {
            if (node?.CodeFile == null)
            {
                return;
            }

            var dte = this.GetDte();
            var project = dte.GetActiveProject();

            //var solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            //IVsHierarchy hierarchy;
            //solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);

            var projectDirectory = project.GetDirectory();
            var fullPath = Path.Combine(projectDirectory ?? string.Empty, node.CodeFile.RelativePathWithoutTargetFramework);

            node.CodeFile.Code.NamespaceName = project.GetDefaultNamespace() + node.CodeFile.AdditionalNamespace;
            node.CodeFile.Code.Classes = new List<Class> { node.Class };

            if (node.Method != null)
            {
                node.CodeFile.Code.Classes[0].Methods = new List<Method> { node.Method };
            }

            node.CodeFile.SaveTo(fullPath);

            project.AddItemFromFile(fullPath);

            dte.OpenFileAsText(fullPath);
        }

        private void TreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = sender as TreeViewItem;
            var node = item?.Header as Node;

            AddItem(node);
        }

        private void ShowRepositoriesButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var package = OneCodePackage.Instance;
                var window = await package.ShowToolWindowAsync(typeof(RepositoriesWindow), 0, true, package.DisposalToken);
                window = await package.ShowToolWindowAsync(typeof(RepositoriesWindow), 0, true, package.DisposalToken);
                if (window?.Frame == null)
                {
                    throw new NotSupportedException("Cannot create tool window");
                }
            });
        }
    }
}