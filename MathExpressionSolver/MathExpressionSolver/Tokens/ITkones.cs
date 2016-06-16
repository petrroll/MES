using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    #region ITokens
    public interface IToken<out T>
    {
        T ReturnValue();
    }

    public interface IUnToken<out T> : IToken<T>
    {
        IToken<T> Child { get; }
    }

    public interface IBinToken<out T> : IToken<T>
    {
        IToken<T> LeftChild { get; }
        IToken<T> RightChild { get; }
    }

    public interface IChildrenToken<out T> : IToken<T>
    {
        IReadOnlyList<IToken<T>> Children { get; }
    }

    public interface ICustFuncToken<T> : IChildrenToken<T>
    {
        IToken<T> FuncTopToken { get; }
    }

    public interface IArgumentToken<T> : IToken<T>
    {
        ICustFuncToken<T> CustFunction { get; }
        int ArgID { get; }
    }
    #endregion

    #region IFactorableTokens
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

    public interface IFactorableIChildrenToken<T> : IChildrenToken<T>, IFactorableToken<T>
    {
        IToken<T>[] MutChildren { get; }
    }

    public interface IFactorableBracketsToken<T> : IFactorableIChildrenToken<T>
    {
        //TODO: Consider removing setter
        IFactorableToken<T>[][] BracketedTokens { get; set; }
    }

    public interface IFactorableCustFuncToken<T> : IFactorableBracketsToken<T>, ICustFuncToken<T>
    {
        IToken<T> MutFuncTopToken { get; set; }
    }
    #endregion

}
