using System.Collections.Generic;

namespace OneCode.ReSharperExtension.ContextActions.Services
{
    public interface IParameterProvider
    {
        IEnumerable<string> GetParameters(string constructorString);
    }
}
