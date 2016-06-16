using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public interface IToken<T>
    {
        T ReturnValue();
    }

    public interface IUnToken<T> : IToken<T>
    {
        IToken<T> Child { get; }
    }

    public interface IBinToken<T> : IToken<T>
    {
        IToken<T> LeftChild { get; }
        IToken<T> RightChild { get; }
    }


    public interface IChildrenToken<T> : IToken<T>
    {
        IToken<T>[] Children { get; }
    }

    public interface ICustFuncToken<T> : IChildrenToken<T>
    {
        IToken<T> FuncTopToken { get; set; }
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

    public interface IFactorableUnToken<T> : IUnToken<T>, IFactorableToken<T>
    {
        IToken<T> MutChild { get; set; }
    }

    public interface IFactorableBinToken<T> : IBinToken<T>, IFactorableToken<T>
    {
        IToken<T> MutLeftChild { get; set; }
        IToken<T> MutRightChild { get; set; }
    }

    public interface IFactorableBracketsToken<T> : IFactorableToken<T>, IChildrenToken<T>
    {
        IFactorableToken<T>[][] BracketedTokens { get; set; }
    }

    public interface IFactorableCustFuncToken<T> : IFactorableBracketsToken<T>, ICustFuncToken<T>
    {

    }

}
