using System.Collections.Generic;

namespace OneCode.Shared
{
    public sealed class Class : CodeObject
    {
        public bool IsStatic { get; set; }

        public List<Method> Methods { get; set; } = new List<Method>();
    }
}
