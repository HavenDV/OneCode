using System;
using System.IO;

#nullable enable

namespace OneCode.Shared
{
    public sealed class CodeFile
    {
        public string FullPath { get; }
        public Repository? Repository { get; }

        public Code Code { get; private set; }
        public string RelativePath { get; } = string.Empty;
        public string TargetFramework { get; } = string.Empty;
        public string RelativeFolder { get; } = string.Empty;
        public string RelativePathWithoutTargetFramework { get; } = string.Empty;
        public string RelativeFolderWithoutTargetFramework { get; } = string.Empty;
        public string AdditionalNamespace { get; } = string.Empty;

        public CodeFile(string path, Repository? repository = null)
        {
            FullPath = path;
            Repository = repository;

            Code = new Code(File.ReadAllText(path), this);

            var baseFolder = repository?.Settings?.Folder;
            if (string.IsNullOrWhiteSpace(baseFolder))
            {
                return;
            }

            RelativePath = path.Replace(baseFolder, string.Empty).TrimStart('\\', '/');
            var index = RelativePath.IndexOfAny(new[] { '\\', '/' });
            RelativePathWithoutTargetFramework = index > 0
                ? RelativePath.Substring(index + 1)
                : RelativePath;
            TargetFramework = RelativePath.Replace(RelativePathWithoutTargetFramework, string.Empty).TrimEnd('\\', '/');
            RelativeFolder = Path.GetDirectoryName(RelativePath) ?? string.Empty;
            RelativeFolderWithoutTargetFramework = !string.IsNullOrWhiteSpace(TargetFramework)
                ? RelativeFolder.Replace(TargetFramework, string.Empty).TrimStart('\\', '/')
                : RelativeFolder;
            AdditionalNamespace = string.IsNullOrWhiteSpace(RelativeFolderWithoutTargetFramework)
                ? string.Empty
                : $".{RelativeFolderWithoutTargetFramework.Replace('\\', '.').Replace('/', '.')}";
        }

        public CodeFile Merge(CodeFile other)
        {
            other = other ?? throw new ArgumentNullException(nameof(other));
            var code = other.Code ?? throw new ArgumentException("other.Code is null", nameof(other));

            Code = Code.Merge(code);

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
                new CodeFile(path)
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
    }
}
