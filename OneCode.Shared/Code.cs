using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

#nullable enable

namespace OneCode.Shared
{
    public sealed class Code
    {
        public string FullText { get; private set; }
        public CodeFile? CodeFile { get; }

        public string? NamespaceName { get; set; }
        public List<Class> Classes { get; set; }

        public Code(string text, CodeFile? codeFile = null)
        {
            FullText = text;
            CodeFile = codeFile;

            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            var root = syntaxTree.GetRoot();

            NamespaceName = root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault()
                ?.Name.ToString();
            Classes = root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Select(syntax => Class.FromSyntax(syntax, this))
                .ToList();
        }

        public Code Merge(Code other)
        {
            FullText = other.FullText;
            Classes = Classes
                .Concat(other.Classes)
                .Distinct()
                .ToList();

            return this;
        }

        public string Save()
        {
            var tree = CSharpSyntaxTree.ParseText(FullText);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var namespaceSyntax = root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();

            if (namespaceSyntax != null)
            {
                root = root.ReplaceNode(namespaceSyntax, namespaceSyntax.WithName(IdentifierName(NamespaceName + Environment.NewLine)));
            }

            var methods = Classes.SelectMany(i => i.Methods).ToList();
            var rewriter = new MethodsRewriter(methods.Select(i => i.Name).Concat(methods.SelectMany(i => i.Dependencies)).ToArray());
            var result = rewriter.Visit(root);

            return result.ToFullString();
        }
    }
}
