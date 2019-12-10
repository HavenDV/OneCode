using System.IO;

namespace OneCode.Shared.Utilities
{
    /// <summary>
    /// A type that can be used to make string work with the <see langword="operator / "/>(uses <see cref="Path.Combine(string, string)"/> internally)<br/>
    /// Supports implicit conversion from <see langword="string"/>/to <see langword="string"/>
    /// </summary>
    public class PathString
    {
        public string Value { get; }

        public PathString(string value)
        {
            Value = value;
        }

        public static implicit operator PathString(string value)
        {
            return new PathString(value);
        }

        public static implicit operator string(PathString value)
        {
            return value.Value;
        }

        public static PathString operator /(PathString left, string right)
        {
            return new PathString(Path.Combine(left.Value, right));
        }
    }
}
