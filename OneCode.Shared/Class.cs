using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OneCode.Shared.Utilities;

#nullable enable

namespace OneCode.Shared
{
    public sealed class Class : CodeObject
    {
        public Code? Code { get; set; }

        public bool IsStatic { get; set; }
        public List<Method> Methods { get; set; } = new List<Method>();

        public static Class FromSyntax(ClassDeclarationSyntax syntax, Code? code = null)
        {
            var @class = new Class
            {
                Code = code,
                Name = syntax.Identifier.Text,
                FullText = syntax.ToFullString(),
                Version = XmlUtilities.GetVersion(syntax.Modifiers.ToFullString()),
                Dependencies = XmlUtilities.GetDependencies(syntax.Modifiers.ToFullString()),
                IsStatic = syntax.Modifiers.Any(SyntaxKind.StaticKeyword)
            };

            @class.Methods = syntax
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Select(methodSyntax => Method.FromSyntax(methodSyntax, @class))
                .ToList();

            return @class;
        }
    }
}
