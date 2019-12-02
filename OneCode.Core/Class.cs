using System.Collections.Generic;

namespace OneCode.Core
{
    public class Class : CodeObject
    {
        public bool IsStatic { get; set; }

        public List<Method> Methods { get; set; } = new List<Method>();
    }
}
