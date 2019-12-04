namespace OneCode.Core
{
    public sealed class Method : CodeObject
    {
        public bool IsStatic { get; set; }
        public bool IsExtension { get; set; }
    }
}
