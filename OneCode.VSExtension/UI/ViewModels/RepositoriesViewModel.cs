using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.PlatformUI;
using OneCode.Core;
using OneCode.VsExtension.Properties;
using MessageBox = System.Windows.Forms.MessageBox;

namespace OneCode.VsExtension.UI.ViewModels
{
    public class RepositoriesViewModel
    {
        private Repositories Model { get; }

        public ObservableCollection<Repository> Values { get; set; }

        public DelegateCommand<Repository> RemoveCommand { get; }
        public DelegateCommand AddCommand { get; }
        public DelegateCommand<Repository> EditCommand { get; }

        public RepositoriesViewModel(Repositories model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));

            Values = model.Values;

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

            if (MessageBox.Show(
                    $"Are you sure you want to delete \"{repository?.Folder}\"", 
                    "Question", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            Model.Remove(repository);

            Settings.Default.RepositoryPath = string.Join(";", Model.Values.Select(i => i.Folder));
            Settings.Default.Save();
        }

        private static void OnEdit(Repository repository)
        {
            if (repository == null)
            {
                return;
            }
            
            MessageBox.Show($"Edit {repository.Folder}");
        }

        private void OnAdd()
        {
            using var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK ||
                string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                return;
            }

            var path = dialog.SelectedPath;

            Model.Load(path);

            Settings.Default.RepositoryPath = string.Join(";", Model.Values.Select(i => i.Folder));
            Settings.Default.Save();
        }
    }
}
