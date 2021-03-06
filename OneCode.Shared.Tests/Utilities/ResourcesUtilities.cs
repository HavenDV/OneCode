using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OneCode.Shared.Tests.Utilities
{
    public static class ResourcesUtilities
    {
        /// <summary>
        /// <![CDATA[Version: 1.0.0.0]]>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Stream ReadFileAsStream(string name, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

            return assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"\"{name}\" is not found in embedded resources");
        }

        /// <summary>
        /// <![CDATA[Version: 1.0.0.0]]>
        /// <![CDATA[Dependency: ReadFileAsStream(string name, Assembly assembly = null)]]>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string ReadFileAsString(string name, Assembly? assembly = null)
        {
            using var stream = ReadFileAsStream(name, assembly);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}