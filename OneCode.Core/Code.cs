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

        public string? NamespaceName { get; set; } = string.Empty;
        public string FullText { get; set; } = string.Empty;

        public List<Class> Classes { get; set; } = new List<Class>();

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

        #region Static methods

        public static Code Load(string text)
        {
            var file = new Code();
            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            var root = syntaxTree.GetRoot();

            file.FullText = text;
            file.NamespaceName = root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault()
                ?.Name.ToString();
            file.Classes = root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Select(classSyntax => new Class
                {
                    Name = classSyntax.Identifier.Text,
                    FullText = classSyntax.ToFullString(),
                    Version = GetVersion(classSyntax.Modifiers.ToFullString()),
                    Dependencies = GetDependencies(classSyntax.Modifiers.ToFullString()),
                    IsStatic = classSyntax.Modifiers.Any(SyntaxKind.StaticKeyword),
                    Methods = classSyntax
                        .DescendantNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .Select(methodSyntax => new Method
                        {
                            Name = methodSyntax.Identifier.Text + methodSyntax.ParameterList,
                            FullText = methodSyntax.ToFullString(),
                            Version = GetVersion(methodSyntax.Modifiers.ToFullString()),
                            Dependencies = GetDependencies(methodSyntax.Modifiers.ToFullString()),
                            IsStatic = methodSyntax.Modifiers.Any(SyntaxKind.StaticKeyword),
                            IsExtension = methodSyntax.ParameterList.Parameters.ToString().StartsWith("this"),
                        })
                        .ToList()
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
            return Version.TryParse(GetVersionText(modifiersText), out var result)
                ? result
                : Version.Parse("1.0.0.0");
        }

        #endregion
    }
}
