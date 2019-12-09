using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OneCode.Shared.Settings
{
    public sealed class OneCodeSettings
    {
        public List<RepositorySettings> RepositoriesSettings { get; set; } = new List<RepositorySettings>();

        public static string DefaultDirectory => Directory.CreateDirectory(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create),
                "OneCode")).FullName;

        public static string DefaultPath => Path.Combine(DefaultDirectory, "settings.json");

        public static OneCodeSettings DefaultSettings {
            get => File.Exists(DefaultPath)
                ? JsonConvert.DeserializeObject<OneCodeSettings>(File.ReadAllText(DefaultPath))
                : new OneCodeSettings();
            set => File.WriteAllText(DefaultPath, JsonConvert.SerializeObject(value, Formatting.Indented));
        }
    }
}
