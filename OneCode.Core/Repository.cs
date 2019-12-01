using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OneCode.Core
{
    public static class Repository
    {
        public static Dictionary<string, List<Method>> Load(string path)
        {
            return Directory
                .EnumerateFiles(path, "*.cs", SearchOption.AllDirectories)
                .Where(value => !value.EndsWith("AssemblyInfo.cs"))
                .ToDictionary(
                    filePath => filePath.Replace(path, string.Empty),
                    GetMethodsFromPath);
        }

        public static List<Method> GetMethods(string text)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            var root = syntaxTree.GetRoot();
           
            return root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Select(syntax => new Method {
                    Name = syntax.Identifier.Text + syntax.ParameterList,
                    FullText = syntax.ToFullString(),
                    Version = GetVersion(syntax.Modifiers.ToFullString()),
                })
                .ToList();
        }

        public static string GetVersionText(string text)
        {
            const string prefix = "Version: ";
            if (!text.Contains(prefix))
            {
                return "1.0.0.0";
            }

            var index = text.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            index += prefix.Length;

            var index2 = text.IndexOf('\r', index + 1);

            return text.Substring(index, index2 - index);
        }

        public static Version GetVersion(string text)
        {
            return Version.Parse(GetVersionText(text));
        }

        public static List<Method> GetMethodsFromPath(string path)
        {
            var text = File.ReadAllText(path);

            return GetMethods(text);
        }
    }
}
