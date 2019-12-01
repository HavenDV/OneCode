using System.IO;

namespace OneCode.Core
{
    public class CodeFile
    {
        public string FullPath { get; set; }
        public string RelativePath { get; set; }

        public Code Code { get; set; }

        public CodeFile Merge(CodeFile other)
        {
            Code = Code.Merge(other.Code);

            return this;
        }

        public void Save()
        {
            File.WriteAllText(FullPath, Code.Save());
        }

        public static CodeFile Load(string path, string baseFolder = null)
        {
            return new CodeFile
            {
                FullPath = path,
                RelativePath = baseFolder != null ? path.Replace(baseFolder, string.Empty) : string.Empty,
                Code = Code.Load(File.ReadAllText(path))
            };
        }
    }
}
