using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    #region ITokens
    public interface IToken<T> : IClonable<IToken<T>>
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
        IToken<T> MutChild { set; }
    }

    public interface IFactorableBinToken<T> : IBinToken<T>, IFactorableToken<T>
    {
        IToken<T> MutLeftChild { set; }
        IToken<T> MutRightChild { set; }
    }

    public interface IFactorableBracketsToken<T> : IChildrenToken<T>, IFactorableToken<T>
    {
        void SetChild(int index, IToken<T> child);
        //TODO: Consider removing setter
        IFactorableToken<T>[][] BracketedTokens { get; set; }
    }

    public interface IFactorableCustFuncToken<T> : IFactorableBracketsToken<T>, ICustFuncToken<T>
    {
        IToken<T> MutFuncTopToken { get; set; }
        IFactorableUnToken<T>[] MutArgumentTokens { get; set; }
    }
    #endregion

}
