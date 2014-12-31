﻿using System.Collections.Generic;

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

    public interface IBiToken<T> : IToken<T>
    {
        IToken<T> LeftChild { get; set; }
        IToken<T> RightChild { get; set; }
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