using System;
using System.Collections.ObjectModel;
using System.Linq;
using OneCode.Core.Settings;

namespace OneCode.Core
{
    public sealed class Repositories
    {
        #region Properties

        public ObservableCollection<Repository> Values { get; set; } = new ObservableCollection<Repository>(); 
        
        public OneCodeSettings? Settings { get; set; }

        #endregion

        #region Events

        public event EventHandler? Changed;

        private void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Public methods

        public void Add(RepositorySettings settings)
        {
            Settings = Settings ?? throw new InvalidOperationException("Repositories is not loaded");

            Values.Add(new Repository(settings));
            Settings.RepositoriesSettings.Add(settings);

            OnChanged();
        }

        public void Load(OneCodeSettings? settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            foreach (var repositorySettings in settings.RepositoriesSettings)
            {
                Values.Add(new Repository(repositorySettings));
            }

            OnChanged();
        }

        public void Remove(RepositorySettings settings)
        {
            Settings = Settings ?? throw new InvalidOperationException("Repositories is not loaded");

            var repository = Values.FirstOrDefault(i => i.Settings == settings);
            if (repository == null)
            {
                return;
            }

            Values.Remove(repository);
            Settings.RepositoriesSettings.Remove(settings);

            OnChanged();
        }

        public void Reload()
        {
            Settings = Settings ?? throw new InvalidOperationException("Repositories is not loaded");

            Values.Clear();

            Load(Settings);
        }

        #endregion
    }
}
