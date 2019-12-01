using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneCode.Core.Tests.Utilities
{
    public static class ResourcesUtilities
    {
        public static string ReadFile(string name, Assembly assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

            using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"\"{name}\" is not found in embedded resources");
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}