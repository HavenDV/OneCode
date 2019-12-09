using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.PlatformUI;
using OneCode.Shared;
using OneCode.VsExtension.Services;
using OneCode.VsExtension.UI.Controls;
using OneCode.VsExtension.Utilities;

namespace OneCode.VsExtension.UI.ViewModels
{
    public sealed class OneCodeViewModel
    {
        private RepositoriesService RepositoriesService { get; }
        private ExceptionsService ExceptionsService { get; }
        private Repositories Repositories => RepositoriesService.Repositories;

        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node>();

        public DelegateCommand UpdateCommand { get; }
        public DelegateCommand ShowRepositoriesCommand { get; }
        public DelegateCommand ShowExceptionsCommand { get; }
        public DelegateCommand<Node> AddItemCommand { get; }

        public OneCodeViewModel(RepositoriesService repositoriesService, ExceptionsService exceptionsService)
        {
            RepositoriesService = repositoriesService ?? throw new ArgumentNullException(nameof(repositoriesService));
            ExceptionsService = exceptionsService ?? throw new ArgumentNullException(nameof(exceptionsService));

            Repositories.Changed += (sender, args) => RefreshTree(Repositories);

            UpdateCommand = new DelegateCommand(OnUpdate);
            ShowRepositoriesCommand = new DelegateCommand(OnShowRepositories);
            ShowExceptionsCommand = new DelegateCommand(OnShowExceptions);
            AddItemCommand = new DelegateCommand<Node>(OnAddItem);

            RefreshTree(Repositories);
        }

        private void OnUpdate()
        {
            try
            {
                Repositories.Reload();
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }

        private void OnShowRepositories()
        {
            try
            {
                new RepositoriesViewModel(RepositoriesService, ExceptionsService)
                .ShowAsDialog<RepositoriesControl>(
                    "OneCode Repositories", 400, 400);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }

        private void OnShowExceptions()
        {
            try
            {
                new ExceptionsViewModel(ExceptionsService)
                    .ShowAsDialog<ExceptionsControl>(
                        "OneCode Exceptions", 400, 400);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }

        private void OnAddItem(Node node)
        {
            if (node == null)
            {
                return;
            }

            try
            {
                RepositoriesService.AddProjectItem(node.CodeFile, node.Class, node.Method, true);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }

        private void RefreshTree(Repositories repositories)
        {
            try
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
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
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
