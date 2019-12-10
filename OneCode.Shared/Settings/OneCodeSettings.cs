using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OneCode.Shared.Utilities;

#nullable enable

namespace OneCode.Shared.Settings
{
    public sealed class OneCodeSettings
    {
        public List<RepositorySettings> RepositoriesSettings { get; set; } = new List<RepositorySettings>();

        public static PathString LocalApplicationDataDirectory => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
        public static PathString DefaultDirectory => Directory.CreateDirectory(LocalApplicationDataDirectory / "OneCode").FullName;
        public static PathString DefaultPath => DefaultDirectory / "settings.json";

        public static OneCodeSettings DefaultSettings {
            get => File.Exists(DefaultPath)
                ? JsonConvert.DeserializeObject<OneCodeSettings>(File.ReadAllText(DefaultPath))
                : new OneCodeSettings();
            set => File.WriteAllText(DefaultPath, JsonConvert.SerializeObject(value, Formatting.Indented));
        }
    }
}
