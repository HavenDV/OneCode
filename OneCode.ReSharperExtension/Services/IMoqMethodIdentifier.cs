using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace OneCode.ReSharperExtension.Services
{
    public interface IMoqMethodIdentifier
    {
        bool IsMoqSetupMethod([CanBeNull]  IInvocationExpression invocationExpression);
        bool IsMoqVerifyMethod([CanBeNull]  IInvocationExpression invocationExpression);
        bool IsMoqReturnMethod([CanBeNull]  IInvocationExpression invocationExpression);
        bool IsMoqCallbackMethod([CanBeNull]  IInvocationExpression invocationExpression);
    }
}
