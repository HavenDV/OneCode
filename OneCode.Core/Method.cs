using System;
using System.Collections.Generic;

namespace OneCode.Core
{
    public class Method
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public List<string> Dependencies { get; set; } = new List<string>();
        public bool IsStatic { get; set; }
        public bool IsExtension { get; set; }
        public string FullText { get; set; }
    }
}
