using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.PlatformUI;
using OneCode.Core;
using OneCode.VsExtension.UI.Controls;
using OneCode.VsExtension.Utilities;

namespace OneCode.VsExtension.UI.ViewModels
{
    public sealed class OneCodeViewModel
    {
        private Repositories Model { get; }

        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node>();

        public DelegateCommand UpdateCommand { get; }
        public DelegateCommand ShowRepositoriesCommand { get; }
        public DelegateCommand ShowExceptionsCommand { get; }
        public DelegateCommand<Node> AddItemCommand { get; }

        public OneCodeViewModel(Repositories model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Model.Changed += (sender, args) => RefreshTree(Model);

            UpdateCommand = new DelegateCommand(OnUpdate);
            ShowRepositoriesCommand = new DelegateCommand(OnShowRepositories);
            ShowExceptionsCommand = new DelegateCommand(OnShowExceptions);
            AddItemCommand = new DelegateCommand<Node>(OnAddItem);

            RefreshTree(Model);
        }

        private void OnUpdate()
        {
            Model.Reload();
        }

        private void OnShowRepositories()
        {
            new RepositoriesViewModel(Model)
                .ShowAsDialog<RepositoriesControl>(
                    "OneCode Repositories", 400, 400);
        }

        private static void OnShowExceptions()
        {
            new ExceptionsViewModel(OneCodePackage.Instance.Exceptions)
                .ShowAsDialog<ExceptionsControl>(
                    "OneCode Exceptions", 400, 400);
        }

        private static void OnAddItem(Node node)
        {
            if (node == null)
            {
                return;
            }

            OneCodePackage.AddItem(node.CodeFile, node.Class, node.Method, true);
        }

        private void RefreshTree(Repositories repositories)
        {
            Nodes.Clear();

            Nodes.Add(new Node
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
            });
            Nodes.Add(new Node
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
            });
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public Method Method { get; set; }
        public Class Class { get; set; }
        public CodeFile CodeFile { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }
    }
}
