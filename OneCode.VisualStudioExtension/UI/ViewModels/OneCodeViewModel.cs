using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.PlatformUI;
using OneCode.Shared;
using OneCode.VsExtension.Services;
using OneCode.VsExtension.UI.Controls;
using OneCode.VsExtension.Utilities;

#nullable enable

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
        public DelegateCommand<Node> OpenFileCommand { get; }
        public DelegateCommand<Node> OpenFolderCommand { get; }
        public DelegateCommand<Node> OpenSolutionCommand { get; }

        public OneCodeViewModel(RepositoriesService? repositoriesService, ExceptionsService? exceptionsService)
        {
            RepositoriesService = repositoriesService ?? throw new ArgumentNullException(nameof(repositoriesService));
            ExceptionsService = exceptionsService ?? throw new ArgumentNullException(nameof(exceptionsService));

            Repositories.Changed += (sender, args) => RefreshTree(Repositories);

            UpdateCommand = new DelegateCommand(OnUpdate);
            ShowRepositoriesCommand = new DelegateCommand(OnShowRepositories);
            ShowExceptionsCommand = new DelegateCommand(OnShowExceptions);
            AddItemCommand = new DelegateCommand<Node>(OnAddItem);
            OpenFileCommand = new DelegateCommand<Node>(OnOpenFile);
            OpenFolderCommand = new DelegateCommand<Node>(OnOpenFolder);
            OpenSolutionCommand = new DelegateCommand<Node>(OnOpenSolution);

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

        private void OnOpenFile(Node node)
        {
            if (node == null)
            {
                return;
            }

            try
            {
                var path = (node.Method?.Class ?? node.Class)?.Code?.CodeFile?.FullPath;
                if (path == null)
                {
                    return;
                }

                RepositoriesService.OpenFile(path);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }

        private void OnOpenFolder(Node node)
        {
            if (node == null)
            {
                return;
            }

            try
            {
                var path = (node.Method?.Class ?? node.Class)?.Code?.CodeFile?.Repository?.Settings?.Folder;
                if (path == null)
                {
                    return;
                }

                Process.Start(path);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }

        private void OnOpenSolution(Node node)
        {
            if (node == null)
            {
                return;
            }

            try
            {
                var path = (node.Method?.Class ?? node.Class)?.Code?.CodeFile?.Repository?.Settings?.Folder;
                if (path == null)
                {
                    return;
                }

                var solutionPath = Directory.EnumerateFiles(path, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (solutionPath == null)
                {
                    return;
                }

                Process.Start(solutionPath);
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
                                .Where(@class => @class.IsStatic)
                                .SelectMany(@class => @class.Methods)
                                .Where(method => method.IsStatic)
                                .Select(method => new Node
                                {
                                    Name = $"{method.Class?.Name}.{method.Name}",
                                    Method = method,
                                    Class = method.Class,
                                    CodeFile = method.Class?.Code?.CodeFile,
                                })),
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
        public string? Name { get; set; }
        public Method? Method { get; set; }
        public Class? Class { get; set; }
        public CodeFile? CodeFile { get; set; }
        public ObservableCollection<Node>? Nodes { get; set; }
    }
}
