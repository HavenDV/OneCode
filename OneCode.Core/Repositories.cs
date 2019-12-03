using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OneCode.Core
{
    public class Repositories
    {
        public ObservableCollection<Repository> Values { get; set; } = new ObservableCollection<Repository>();

        public event EventHandler? Changed;

        public void Load(string path)
        {
            if (Values.Any(repository => repository.Folder == path))
            {
                return;
            }

            Values.Add(Repository.Load(path));

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Load(List<string> paths)
        {
            foreach (var path in paths)
            {
                Load(path);
            }
        }

        public void Reload()
        {
            var paths = Values
                .Select(repository => repository.Folder)
                .ToList();

            Values.Clear();

            Load(paths);
        }
    }
}
