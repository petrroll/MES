using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public interface IToken<T>
    {
        T ReturnValue();
    }

    public interface IUnToken<T> : IToken<T>
    {
        IToken<T> Child { get; set; }
    }

    public interface IBinToken<T> : IToken<T>
    {
        IToken<T> LeftChild { get; set; }
        IToken<T> RightChild { get; set; }
    }

    public interface IChildrenToken<T> : IToken<T>
    {
        IToken<T>[] Children { get; }
    }

    public interface ICustFuncToken<T> : IChildrenToken<T>
    {
        IToken<T> FuncTopToken { get; set; }
        T GetArgValue(int ArgID);
    }

    public interface IArgumentToken<T> : IToken<T>
    {
        ICustFuncToken<T> CustFunction { get; set; }
        int ArgID { get; set; }
    }

    public interface IFactorableToken<T> : IToken<T>
    {
        int Priority { get; }
        TokenType Type { get; }
    }

    public interface IFactorableBracketsToken<T> : IFactorableToken<T>, IChildrenToken<T>
    {
        IFactorableToken<T>[][] BracketedTokens { get; set; }
    }

}
