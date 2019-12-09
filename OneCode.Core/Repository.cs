using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OneCode.Core.Settings;

namespace OneCode.Core
{
    public sealed class Repository
    {
        public RepositorySettings Settings { get; set; }
        public List<CodeFile> Files { get; set; }

        public Repository(RepositorySettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));

            Files = Directory
                .EnumerateFiles(settings.Folder, "*.cs", SearchOption.AllDirectories)
                .Where(value => !value.EndsWith("AssemblyInfo.cs"))
                .Select(path => CodeFile.Load(path, settings.Folder))
                .ToList();
        }
    }
}
