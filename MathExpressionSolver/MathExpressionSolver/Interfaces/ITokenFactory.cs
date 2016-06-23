using MathExpressionSolver.Tokens;
using System.Collections.Generic;

namespace MathExpressionSolver.Interfaces
{
    public interface ITokenFactory<T>
    {
        Dictionary<string, T> CustomVariables { get; set; }
        IFactorableBracketsToken<T> CreateBrackets(IFactorableToken<T>[][] arguments);
        IFactorableBracketsToken<T> CreateFunction(string s, IFactorableToken<T>[][] arguments);
        IFactorableToken<T> CreateValue(string s);
        IFactorableToken<T> CreateVariable(string s);
        IFactorableToken<T> CreateOperator(string s);

    }

    public interface ICustomFunctionsAwareTokenFactory<T> : ITokenFactory<T>
    {
        string[] ArgsArray { set; }
        IFactorableCustFuncToken<T> CustomFunction { set; }
        Dictionary<string, IFactorableCustFuncToken<T>> CustomFunctions { get; set; }

        void Clear();
    }
}
