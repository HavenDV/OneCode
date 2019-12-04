using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OneCode.Core
{
    public sealed class Repositories
    {
        #region Properties

        public ObservableCollection<Repository> Values { get; set; } = new ObservableCollection<Repository>();

        #endregion

        #region Events

        public event EventHandler? Changed;

        private void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Public methods

        public void Load(string path, bool raiseChanged = true)
        {
            if (Values.Any(repository => repository.Folder == path))
            {
                return;
            }

            Values.Add(Repository.Load(path));

            if (raiseChanged)
            {
                OnChanged();
            }
        }

        public void Load(List<string> paths)
        {
            foreach (var path in paths)
            {
                Load(path, false);
            }

            OnChanged();
        }

        public void Remove(Repository repository)
        {
            Values.Remove(repository);

            OnChanged();
        }

        public void Reload()
        {
            var paths = Values
                .Select(repository => repository.Folder)
                .ToList();

            Values.Clear();

            Load(paths);
        }

        #endregion
    }
}
