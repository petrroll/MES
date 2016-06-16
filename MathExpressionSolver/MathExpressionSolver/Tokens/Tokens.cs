﻿using System;
using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public enum TokenType { BinOperator, Function, Element, Brackets };

    public abstract class Token<T> : IFactorableToken<T>
    {
        virtual public int Priority { get; protected set; }
        virtual public TokenType Type { get; protected set; }

        public IToken<T>[] MutChildren { get; protected set; }
        public IReadOnlyList<IToken<T>> Children { get { return Array.AsReadOnly<IToken<T>>(MutChildren); } } 
        abstract public T ReturnValue();
    }

    public abstract class UnToken<T> : Token<T>, IFactorableUnToken<T>
    {
        public IToken<T> Child { get { return Children[0]; } }
        public IToken<T> MutChild { get { return Children[0]; } set { MutChildren[0] = value; } }

        protected UnToken()
        {
            Priority = int.MaxValue;
            MutChildren = new IToken<T>[1];
        }

        public override T ReturnValue()
        {
            return default(T);
        }
    }


    public class ItemToken<T> : UnToken<T>
    {
        new public T Child { get { return Children[0]; } set { Children[0] = value; } }
        new public T[] Children { get; protected set; }

        public ItemToken()
        {
            Type = TokenType.Element;
            Children = new T[1];
        }

        public override T ReturnValue()
        {
            return Child;
        }

        public override string ToString()
        {
            return Child.ToString();
        }
    }

    public class ArgToken<T> : Token<T>, IArgumentToken<T>
    {
        public ICustFuncToken<T> CustFunction { get; set; }
        public int ArgID { get; set; }

        public ArgToken()
        {
            MutChildren = new IToken<T>[0];
            Type = TokenType.Element;
            Priority = int.MaxValue;
        }

        public override T ReturnValue()
        {
            return CustFunction.Children[ArgID].ReturnValue();
        }
    }

    public abstract class BinOpToken<T> : Token<T>, IFactorableBinToken<T>
    {
        public IToken<T> LeftChild { get { return Children[0]; } }
        public IToken<T> RightChild { get { return Children[1]; } }

        public IToken<T> MutLeftChild { get { return Children[0]; } set { MutChildren[0] = value; } }
        public IToken<T> MutRightChild { get { return Children[1]; } set { MutChildren[1] = value; } }

        protected BinOpToken()
        {
            Type = TokenType.BinOperator;
            MutChildren = new IToken<T>[2];
        }

        public override T ReturnValue()
        {
            return default(T);
        }
    }

    public class MinusToken<T> : BinOpToken<T>
    {
        public MinusToken()
        {
            Priority = 2;
        }

        public override T ReturnValue()
        {
            dynamic l = LeftChild.ReturnValue();
            dynamic r = RightChild.ReturnValue();

            return l - r;
        }

        public override string ToString()
        {
            return LeftChild?.ToString() + " - " + RightChild?.ToString();
        }
    }

    public class PlusToken<T> : BinOpToken<T>
    {
        public PlusToken()
        {
            Priority = 2;
        }

        public override T ReturnValue()
        {
            dynamic l = LeftChild.ReturnValue();
            dynamic r = RightChild.ReturnValue();

            return l + r;
        }

        public override string ToString()
        {
            return LeftChild?.ToString() + " + " + RightChild?.ToString();
        }
    }

    public class TimesToken<T> : BinOpToken<T>
    {
        public TimesToken()
        {
            Priority = 3;
        }

        public override T ReturnValue()
        {
            dynamic l = LeftChild.ReturnValue();
            dynamic r = RightChild.ReturnValue();

            return l * r;
        }

        public override string ToString()
        {
            return LeftChild?.ToString() + " * " + RightChild?.ToString();
        }
    }

    public class DivToken<T> : BinOpToken<T>
    {
        public DivToken()
        {
            Priority = 3;
        }

        public override T ReturnValue()
        {
            dynamic l = LeftChild.ReturnValue();
            dynamic r = RightChild.ReturnValue();

            return l / r;
        }

        public override string ToString()
        {
            return LeftChild?.ToString() + " / " + RightChild?.ToString();
        }
    }

    public class GrtToken : BinOpToken<double>
    {
        public GrtToken()
        {
            Priority = 0;
        }

        public override double ReturnValue()
        {
            dynamic l = LeftChild.ReturnValue();
            dynamic r = RightChild.ReturnValue();

            return l > r ? 1 : 0;
        }

        public override string ToString()
        {
            return LeftChild?.ToString() + " >" + RightChild?.ToString();
        }
    }

    public class SmlrToken : BinOpToken<double>
    {
        public SmlrToken()
        {
            Priority = 0;
        }

        public override double ReturnValue()
        {
            dynamic l = LeftChild.ReturnValue();
            dynamic r = RightChild.ReturnValue();

            return l < r ? 1 : 0;
        }

        public override string ToString()
        {
            return LeftChild?.ToString() + " >" + RightChild?.ToString();
        }
    }

    public abstract class FuncToken<T> : Token<T>, IFactorableBracketsToken<T>
    {
        public IFactorableToken<T>[][] BracketedTokens { get; set; }

        protected FuncToken(int arguments)
        {
            Type = TokenType.Function;
            Priority = int.MaxValue;

            MutChildren = new IToken<T>[arguments];
            BracketedTokens = new IFactorableToken<T>[arguments][];
        }

        public override T ReturnValue()
        {
            return default(T);
        }

        public override string ToString()
        {
            return "(" + default(T) + ")";
        }
    }

    public class CustFuncToken<T> : FuncToken<T>, IFactorableCustFuncToken<T>
    {
        public IToken<T> FuncTopToken { get { return MutFuncTopToken; }  }

        public IToken<T> MutFuncTopToken { get; set; }

        public CustFuncToken(int numOfArgs)
            : base(numOfArgs)
        {

        }

        public override T ReturnValue()
        {
            return FuncTopToken.ReturnValue();
        }
    }

    public class BracketToken<T> : FuncToken<T>
    {
        public BracketToken() : base(1)
        {
            Type = TokenType.Brackets;
        }

        public override T ReturnValue()
        {
            return Children[0].ReturnValue();
        }

        public override string ToString()
        {
            return "(" + Children[0]?.ToString() + ")";
        }
    }

    public class ExpToken : FuncToken<double>
    {
        public ExpToken() : base(1)
        {
            Type = TokenType.Function;
        }

        public override double ReturnValue()
        {
            return System.Math.Exp(Children[0].ReturnValue());
        }

        public override string ToString()
        {
            return "exp(" + Children[0]?.ToString() + ")";
        }
    }

    public class LnFunc : FuncToken<double>
    {
        public LnFunc() : base(1)
        {
            Type = TokenType.Function;
        }

        public override double ReturnValue()
        {
            return System.Math.Log(Children[0].ReturnValue());
        }

        public override string ToString()
        {
            return "ln(" + Children[0]?.ToString() + ")";
        }
    }

    public class SqrtFunc : FuncToken<double>
    {
        public SqrtFunc() : base(1)
        {
            Type = TokenType.Function;
        }

        public override double ReturnValue()
        {
            return System.Math.Sqrt(Children[0].ReturnValue());
        }

        public override string ToString()
        {
            return "sqrt(" + Children[0]?.ToString() + ")";
        }
    }

    public class SinFunc : FuncToken<double>
    {
        public SinFunc() : base(1)
        {
            Type = TokenType.Function;
        }

        public override double ReturnValue()
        {
            return System.Math.Sin(Children[0].ReturnValue());
        }

        public override string ToString()
        {
            return "sin(" + Children[0]?.ToString() + ")";
        }
    }

    public class CosFunc : FuncToken<double>
    {
        public CosFunc() : base(1)
        {
            Type = TokenType.Function;
        }

        public override double ReturnValue()
        {
            return System.Math.Cos(Children[0].ReturnValue());
        }

        public override string ToString()
        {
            return "cos(" + Children[0]?.ToString() + ")";
        }
    }

    public class TanFunc : FuncToken<double>
    {
        public TanFunc() : base(1)
        {
            Type = TokenType.Function;
        }

        public override double ReturnValue()
        {
            return System.Math.Tan(Children[0].ReturnValue());
        }

        public override string ToString()
        {
            return "tan(" + Children[0]?.ToString() + ")";
        }
    }

    public class IfToken : FuncToken<double>
    {
        public IfToken() : base(3)
        {
            Type = TokenType.Function;
        }

        public override double ReturnValue()
        {
            return (Children[0].ReturnValue() > 0) ? Children[1].ReturnValue() : Children[2].ReturnValue();
        }

        public override string ToString()
        {
            return "if(" + Children[0]?.ToString() + ";" + Children[1]?.ToString() + ";" + Children[2]?.ToString() + ")";
        }
    }

}
