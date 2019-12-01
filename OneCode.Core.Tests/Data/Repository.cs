using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OneCode.Core
{
    public static class Repository
    {
        /// <summary>
        /// <![CDATA[Version: 1.1.1.1]]>
        /// <![CDATA[Dependency: GetMethods(string path)]]>
        /// <![CDATA[Dependency: GetMethods(string path)]]>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Dictionary<string, string[]> Load(string path)
        {
            return Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories)
                .ToDictionary(
                    filePath => filePath.Replace(path, string.Empty),
                    GetMethods);
        }

        private static string[] GetMethods(string path)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(path);
            var root = syntaxTree.GetRoot();
           
            return root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Select(syntax => syntax.Identifier.Text)
                .ToArray();
        }
    }
}
