using System;
using System.Collections.ObjectModel;
using OneCode.Shared;

namespace OneCode.VsExtension.UI.ViewModels
{
    public sealed class RepositoryViewModel
    {
        private Repository Model { get; }

        public ObservableCollection<CodeFile> Values { get; set; }

        public RepositoryViewModel(Repository model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));

            Values = new ObservableCollection<CodeFile>(Model.Files);
        }
    }
}
