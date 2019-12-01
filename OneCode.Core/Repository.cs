using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OneCode.Core
{
    public static class Repository
    {
        public static Dictionary<string, Dictionary<string, string>> Load(string path)
        {
            return Directory
                .EnumerateFiles(path, "*.cs", SearchOption.AllDirectories)
                .Where(value => !value.EndsWith("AssemblyInfo.cs"))
                .ToDictionary(
                    filePath => filePath.Replace(path, string.Empty),
                    GetMethodsFromPath);
        }

        public static Dictionary<string, string> GetMethods(string text)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            var root = syntaxTree.GetRoot();
           
            return root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToDictionary(
                    syntax => syntax.Identifier.Text + syntax.ParameterList,
                    syntax => syntax.ToFullString());
        }

        public static Dictionary<string, string> GetMethodsFromPath(string path)
        {
            var text = File.ReadAllText(path);

            return GetMethods(text);
        }
    }
}
