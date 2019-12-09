using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace OneCode.ReSharperExtension.Services
{
    public interface IMockedMethodProvider
    {
        IMethod GetMockedMethodFromSetupMethod([CanBeNull] IInvocationExpression invocationExpression);
        TreeNodeCollection<ICSharpArgument>? GetMockedMethodParametersFromSetupMethod(IInvocationExpression invocationExpression);
        IEnumerable<string> GetMockedMethodParameterTypes(IInvocationExpression invocationExpression);
    }
}
