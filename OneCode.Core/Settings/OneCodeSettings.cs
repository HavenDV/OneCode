using System.Collections.Generic;

namespace OneCode.Core.Settings
{
    public sealed class OneCodeSettings
    {
        public List<RepositorySettings> RepositoriesSettings { get; set; } = new List<RepositorySettings>();
    }
}
