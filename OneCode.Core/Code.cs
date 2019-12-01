using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OneCode.Core
{
    public class Code
    {
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

            var rewriter = new MethodsRewriter(Methods.Select(i => i.Name).ToArray());
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
                })
                .ToList();

            return file;
        }

        public static string GetVersionText(string modifiersText)
        {
            const string prefix = "Version: ";
            if (!modifiersText.Contains(prefix))
            {
                return "1.0.0.0";
            }

            var index = modifiersText.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            index += prefix.Length;

            var index2 = modifiersText.IndexOf('\r', index + 1);

            return modifiersText.Substring(index, index2 - index);
        }

        public static Version GetVersion(string modifiersText)
        {
            return Version.Parse(GetVersionText(modifiersText));
        }

        #endregion
    }
}
