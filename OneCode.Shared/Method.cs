using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OneCode.Shared.Utilities;

#nullable enable

namespace OneCode.Shared
{
    public sealed class Method : CodeObject
    {
        public Class? Class { get; set; }

        public bool IsStatic { get; set; }
        public bool IsExtension { get; set; }

        public static Method FromSyntax(MethodDeclarationSyntax syntax, Class? @class = null)
        {
            return new Method
            {
                Class = @class,
                Name = syntax.Identifier.Text + syntax.ParameterList,
                FullText = syntax.ToFullString(),
                Version = XmlUtilities.GetVersion(syntax.Modifiers.ToFullString()),
                Dependencies = XmlUtilities.GetDependencies(syntax.Modifiers.ToFullString()),
                IsStatic = syntax.Modifiers.Any(SyntaxKind.StaticKeyword),
                IsExtension = syntax.ParameterList.Parameters.ToString().StartsWith("this"),
            };
        }
    }
}
