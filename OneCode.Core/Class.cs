using System.Collections.Generic;

namespace OneCode.Core
{
    public sealed class Class : CodeObject
    {
        public bool IsStatic { get; set; }

        public List<Method> Methods { get; set; } = new List<Method>();
    }
}
