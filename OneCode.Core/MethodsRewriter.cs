using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OneCode.Core
{
    internal class MethodsRewriter : CSharpSyntaxRewriter
    {
        private string[] MethodNamesToSave { get; }

        public MethodsRewriter(string[] methodNamesToSave) : base(true)
        {
            MethodNamesToSave = methodNamesToSave;
        }

        public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            return MethodNamesToSave.Contains(node.Identifier.Text + node.ParameterList)
                ? base.VisitMethodDeclaration(node)
                : null;
        }
    }
}