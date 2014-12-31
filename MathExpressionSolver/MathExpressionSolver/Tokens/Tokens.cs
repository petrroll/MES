using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionSolver.Tokens
{
    public enum TokenType { Operator, Function, Element };

    public interface IToken<T>
    {
        T ReturnValue();
        int Priority { get; }
        TokenType Type { get; }
    }

    public abstract class Token<T> : IToken<T>
    {
        virtual public int Priority { get; protected set; }
        virtual public TokenType Type { get; protected set; }
        abstract public T ReturnValue();
    }

    public abstract class UnToken<T> : Token<T>
    {
        public IToken<T> Child { get; set; }
    }

    public class NumToken<T> : UnToken<T>
    {
        public NumToken()
        {
            Priority = int.MaxValue;
            Type = TokenType.Element;
        }

        new public T Child { get; set; }

        public override T ReturnValue()
        {
            return Child;
        }

        public override string ToString()
        {
            return Child.ToString();
        }
    }

    public abstract class BinToken<T> : Token<T>
    {
        public IToken<T> LeftChild { get; set; }
        public IToken<T> RightChild { get; set; }
    }

    public abstract class BinOpToken<T> : BinToken<T>
    {
        public BinOpToken()
        {
            Type = TokenType.Operator;
        }
    }

    public abstract class IntBinOpToken : BinOpToken<int> { }

    public class MinusToken<T> : BinOpToken<T>
    {
        public MinusToken()
        {
            Priority = 2;
            Type = TokenType.Operator;
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
        public PlusToken()
        {
            Priority = 2;
            Type = TokenType.Operator;
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
        public TimesToken()
        {
            Priority = 3;
            Type = TokenType.Operator;
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
        public DivToken()
        {
            Priority = 3;
            Type = TokenType.Operator;
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
