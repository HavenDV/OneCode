using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.PlatformUI;
using OneCode.Core;
using OneCode.VsExtension.Services;
using OneCode.VsExtension.UI.Controls;
using OneCode.VsExtension.Utilities;
using MessageBox = System.Windows.Forms.MessageBox;

namespace OneCode.VsExtension.UI.ViewModels
{
    public sealed class RepositoriesViewModel
    {
        private RepositoriesService RepositoriesService { get; }
        private ExceptionsService ExceptionsService { get; }

        public ObservableCollection<Repository> Values { get; set; }

        public DelegateCommand<Repository> RemoveCommand { get; }
        public DelegateCommand AddCommand { get; }
        public DelegateCommand<Repository> EditCommand { get; }

        public RepositoriesViewModel(RepositoriesService repositoriesService, ExceptionsService exceptionsService)
        {
            RepositoriesService = repositoriesService ?? throw new ArgumentNullException(nameof(repositoriesService));
            ExceptionsService = exceptionsService ?? throw new ArgumentNullException(nameof(exceptionsService));

            Values = RepositoriesService.Repositories.Values;

            RemoveCommand = new DelegateCommand<Repository>(OnRemove);
            AddCommand = new DelegateCommand(OnAdd);
            EditCommand = new DelegateCommand<Repository>(OnEdit);
        }

        private void OnRemove(Repository repository)
        {
            if (repository == null)
            {
                return;
            }

            try
            {
                if (MessageBox.Show(
                        $"Are you sure you want to delete \"{repository.Folder}\"",
                        "Question",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    return;
                }

                RepositoriesService.RemoveAndSaveSettings(repository);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }

        private void OnEdit(Repository repository)
        {
            if (repository == null)
            {
                return;
            }

            try
            {
                new RepositoriesViewModel(RepositoriesService, ExceptionsService)
                    .ShowAsDialog<RepositoriesControl>(
                        $"Edit {repository.Folder}", 400, 400);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }

        private void OnAdd()
        {
            try
            {
                using var dialog = new FolderBrowserDialog();

                if (dialog.ShowDialog() != DialogResult.OK ||
                    string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    return;
                }

                RepositoriesService.AddAndSaveSettings(dialog.SelectedPath);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }
    }
}
