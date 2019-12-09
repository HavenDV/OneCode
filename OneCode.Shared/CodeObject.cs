using System;
using System.Collections.Generic;

namespace OneCode.Shared
{
    public class CodeObject
    {
        public string Name { get; set; } = string.Empty;
        public Version Version { get; set; } = new Version(1, 0, 0, 0);
        public List<string> Dependencies { get; set; } = new List<string>();
        public string FullText { get; set; } = string.Empty;
    }
}
