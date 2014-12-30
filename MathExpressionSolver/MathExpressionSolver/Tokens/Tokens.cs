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

    public abstract class UnIntToken : Token<int> { }


    public class IntToken : UnIntToken
    {
        public IntToken()
        {
            Priority = int.MaxValue;
            Type = TokenType.Element;
        }

        new public int Child { get; set; }

        public override int ReturnValue()
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

    public abstract class BinIntToken : BinToken<int> { }

    public class MinusToken : BinIntToken
    {
        public MinusToken()
        {
            Priority = 2;
            Type = TokenType.Operator;
        }

        public override int ReturnValue()
        {
            return LeftChild.ReturnValue() - RightChild.ReturnValue();
        }

        public override string ToString()
        {
            return LeftChild.ToString() + " - " + RightChild.ToString();
        }
    }

    public class PlusToken : BinIntToken
    {
        public PlusToken()
        {
            Priority = 2;
            Type = TokenType.Operator;
        }

        public override int ReturnValue()
        {
            return LeftChild.ReturnValue() + RightChild.ReturnValue();
        }

        public override string ToString()
        {
            return LeftChild.ToString() + " + " + RightChild.ToString();
        }
    }

    public class TimesToken : BinIntToken
    {
        public TimesToken()
        {
            Priority = 3;
            Type = TokenType.Operator;
        }

        public override int ReturnValue()
        {
            return LeftChild.ReturnValue() * RightChild.ReturnValue();
        }

        public override string ToString()
        {
            return LeftChild.ToString() + " * " + RightChild.ToString();
        }
    }

    public class DivToken : BinIntToken
    {
        public DivToken()
        {
            Priority = 3;
            Type = TokenType.Operator;
        }

        public override int ReturnValue()
        {
            return LeftChild.ReturnValue() / RightChild.ReturnValue();
        }

        public override string ToString()
        {
            return LeftChild.ToString() + " / " + RightChild.ToString();
        }
    }

}
