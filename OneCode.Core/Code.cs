using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OneCode.Core.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OneCode.Core
{
    public class Code
    {
        public const string SpecialPrefix = "<![CDATA[";
        public const string SpecialPostfix = "]]>";

        public string NamespaceName { get; set; }
        public string ClassName { get; set; }
        public string Text { get; set; }

        public List<Method> Methods { get; set; }

        public Code Merge(Code other)
        {
            Text = other.Text;
            Methods = Methods
                .Concat(other.Methods)
                .Distinct()
                .ToList();

            return this;
        }

        public string Save()
        {
            var tree = CSharpSyntaxTree.ParseText(Text);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var namespaceSyntax = root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();

            if (namespaceSyntax != null)
            {
                root = root.ReplaceNode(namespaceSyntax, namespaceSyntax.WithName(IdentifierName(NamespaceName + Environment.NewLine)));
            }

            var rewriter = new MethodsRewriter(Methods.Select(i => i.Name).Concat(Methods.SelectMany(i => i.Dependencies)).ToArray());
            var result = rewriter.Visit(root);

            return result.ToFullString();
        }

        #region Static methods

        public static Code Load(string text)
        {
            var file = new Code();
            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            var root = syntaxTree.GetRoot();

            file.Text = text;
            file.NamespaceName = root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault()
                ?.Name.ToString();
            file.ClassName = root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault()
                ?.Identifier.Text;

            file.Methods = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Select(syntax => new Method
                {
                    Name = syntax.Identifier.Text + syntax.ParameterList,
                    FullText = syntax.ToFullString(),
                    Version = GetVersion(syntax.Modifiers.ToFullString()),
                    Dependencies = GetDependencies(syntax.Modifiers.ToFullString()),
                })
                .ToList();

            return file;
        }

        public static string GetVersionText(string modifiersText)
        {
            return modifiersText.Extract(SpecialPrefix + "Version: ", SpecialPostfix) ?? "1.0.0.0";
        }
        
        public static List<string> GetDependencies(string modifiersText)
        {
            return modifiersText.ExtractAll(SpecialPrefix + "Dependency: ", SpecialPostfix);
        }

        public static Version GetVersion(string modifiersText)
        {
            return Version.Parse(GetVersionText(modifiersText));
        }

        #endregion
    }
}
