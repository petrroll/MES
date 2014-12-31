﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionSolver.Tokens
{
    public enum TokenType { Operator, Function, Element, Brackets };

    public abstract class UnToken<T> : IUnToken<T>
    {
        public IToken<T> Child { get; set; }
        abstract public T ReturnValue();
    }


    public class NumToken<T> : UnToken<T>, IFactorableToken<T>
    {
        virtual public int Priority { get; protected set; }
        virtual public TokenType Type { get; protected set; }
        new public T Child { get; set; }

        public NumToken()
        {
            Priority = int.MaxValue;
            Type = TokenType.Element;
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

    public abstract class BinToken<T> : IBiToken<T>
    {
        abstract public T ReturnValue();
        public IToken<T> LeftChild { get; set; }
        public IToken<T> RightChild { get; set; }
    }

    public abstract class BinOpToken<T> : BinToken<T>, IFactorableToken<T>
    {
        virtual public int Priority { get; protected set; }
        virtual public TokenType Type { get; protected set; }

        public BinOpToken()
        {
            Type = TokenType.Operator;
        }
    }

    public class MinusToken<T> : BinOpToken<T>
    {
        public MinusToken() : base()
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
            return LeftChild.ToString() + " - " + RightChild.ToString();
        }
    }

    public class PlusToken<T> : BinOpToken<T>
    {
        public PlusToken() : base()
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
            return LeftChild.ToString() + " + " + RightChild.ToString();
        }
    }

    public class TimesToken<T> : BinOpToken<T>
    {
        public TimesToken() : base()
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
            return LeftChild.ToString() + " * " + RightChild.ToString();
        }
    }

    public class DivToken<T> : BinOpToken<T>
    {
        public DivToken() : base()
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
            return LeftChild.ToString() + " / " + RightChild.ToString();
        }
    }

}
