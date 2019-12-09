using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.PlatformUI;
using OneCode.Shared.Settings;
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

        public ObservableCollection<RepositorySettings> Values { get; set; }

        public DelegateCommand<RepositorySettings> RemoveCommand { get; }
        public DelegateCommand AddCommand { get; }
        public DelegateCommand<RepositorySettings> EditCommand { get; }

        public RepositoriesViewModel(RepositoriesService repositoriesService, ExceptionsService exceptionsService)
        {
            RepositoriesService = repositoriesService ?? throw new ArgumentNullException(nameof(repositoriesService));
            ExceptionsService = exceptionsService ?? throw new ArgumentNullException(nameof(exceptionsService));

            Values = new ObservableCollection<RepositorySettings>(RepositoriesService.Repositories.Values.Select(i => i.Settings));

            RemoveCommand = new DelegateCommand<RepositorySettings>(OnRemove);
            AddCommand = new DelegateCommand(OnAdd);
            EditCommand = new DelegateCommand<RepositorySettings>(OnEdit);
        }

        private void OnRemove(RepositorySettings settings)
        {
            if (settings == null)
            {
                return;
            }

            try
            {
                if (MessageBox.Show(
                        $"Are you sure you want to delete \"{settings.Folder}\"",
                        "Question",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    return;
                }

                RepositoriesService.RemoveAndSaveSettings(settings);
                Values.Remove(settings);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }

        private void OnEdit(RepositorySettings settings)
        {
            if (settings == null)
            {
                return;
            }

            try
            {
                new RepositoriesViewModel(RepositoriesService, ExceptionsService)
                    .ShowAsDialog<RepositoriesControl>(
                        $"Edit {settings.Folder}", 400, 400);
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

                var settings = new RepositorySettings
                {
                    Folder = dialog.SelectedPath,
                };
                RepositoriesService.AddAndSaveSettings(settings);
                Values.Add(settings);
            }
            catch (Exception exception)
            {
                ExceptionsService.Add(exception);
            }
        }
    }
}
