using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public interface IToken<T>
    {
        T ReturnValue();
        IToken<T>[] Children { get; set; }
    }

    public interface IFactorableToken<T> : IToken<T>
    {
        int Priority { get; }
        TokenType Type { get; }
    }

    public interface IFactorableBracketsToken<T> : IFactorableToken<T>
    {
        IEnumerable<IFactorableToken<T>> BracketedTokens { get; set; }
    }

}
