using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public enum TokenType { BinOperator, Function, Element, Brackets };

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

    public class ExpToken: UnToken<double>, IFactorableBracketsToken<double>
    {
        virtual public int Priority { get; protected set; }
        virtual public TokenType Type { get; protected set; }

        public IEnumerable<IFactorableToken<double>> BracketedTokens { get; set; }

        public ExpToken()
        {
            Priority = int.MaxValue;
            Type = TokenType.Function;
        }

        public override double ReturnValue()
        {
            return System.Math.Exp(Child.ReturnValue());
        }

        public override string ToString()
        {
            return "exp(" + Child.ToString() + ")";
        }
    }

    public class BracketToken<T> : UnToken<T>, IFactorableBracketsToken<T>
    {
        virtual public int Priority { get; protected set; }
        virtual public TokenType Type { get; protected set; }

        public IEnumerable<IFactorableToken<T>> BracketedTokens { get; set; }

        public BracketToken()
        {
            Priority = int.MaxValue;
            Type = TokenType.Brackets;
        }

        public override T ReturnValue()
        {
            return Child.ReturnValue();
        }

        public override string ToString()
        {
            return "(" + Child.ToString() + ")";
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
            Type = TokenType.BinOperator;
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

    public class GrtToken : BinOpToken<double>
    {
        public GrtToken() : base()
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
            return LeftChild.ToString() + " >" + RightChild.ToString();
        }
    }

}
