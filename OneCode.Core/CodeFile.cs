﻿using System;
using System.IO;

namespace OneCode.Core
{
    public sealed class CodeFile
    {
        public string FullPath { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;

        public string TargetFramework { get; set; } = string.Empty;
        public string RelativeFolder { get; set; } = string.Empty;

        public string RelativePathWithoutTargetFramework { get; set; } = string.Empty;
        public string RelativeFolderWithoutTargetFramework { get; set; } = string.Empty;

        public string AdditionalNamespace { get; set; } = string.Empty;

        public Code? Code { get; set; }

        public CodeFile Merge(CodeFile other)
        {
            other = other ?? throw new ArgumentNullException(nameof(other));
            var code = other.Code ?? throw new ArgumentException("other.Code is null", nameof(other));

            Code = Code?.Merge(code);

            return this;
        }

        public void Save()
        {
            File.WriteAllText(FullPath, Code?.Save());
        }

        public void SaveTo(string path)
        {
            if (File.Exists(path))
            {
                Load(path)
                    .Merge(this)
                    .Save();
                return;
            }

            var directory = Path.GetDirectoryName(path);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, Code?.Save());
        }

        public static CodeFile Load(string path, string? baseFolder = null)
        {
            if (string.IsNullOrWhiteSpace(baseFolder))
            {
                return new CodeFile
                {
                    FullPath = path,
                    Code = Code.Load(File.ReadAllText(path))
                };
            }

            var relativePath = path.Replace(baseFolder, string.Empty).TrimStart('\\', '/');
            var index = relativePath.IndexOfAny(new[] { '\\', '/' });
            var relativePathWithoutTargetFramework = index > 0
                ? relativePath.Substring(index + 1)
                : relativePath;
            var targetFramework = relativePath.Replace(relativePathWithoutTargetFramework, string.Empty).TrimEnd('\\', '/');
            var relativeFolder = Path.GetDirectoryName(relativePath) ?? string.Empty;
            var relativeFolderWithoutTargetFramework = !string.IsNullOrWhiteSpace(targetFramework)
                ? relativeFolder.Replace(targetFramework, string.Empty).TrimStart('\\', '/')
                : relativeFolder;
            var additionalNamespace = string.IsNullOrWhiteSpace(relativeFolderWithoutTargetFramework)
                ? string.Empty
                : $".{relativeFolderWithoutTargetFramework.Replace('\\', '.').Replace('/', '.')}";

            return new CodeFile
            {
                FullPath = path,
                RelativePath = relativePath,
                RelativePathWithoutTargetFramework = relativePathWithoutTargetFramework,
                TargetFramework = targetFramework,
                RelativeFolder = relativeFolder,
                RelativeFolderWithoutTargetFramework = relativeFolderWithoutTargetFramework,
                AdditionalNamespace = additionalNamespace,
                Code = Code.Load(File.ReadAllText(path)),
            };
        }
    }
}
