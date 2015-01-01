using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public enum TokenType { BinOperator, Function, Element, Brackets };

    public abstract class UnToken<T> : NToken<T>
    {
        public UnToken()
        {
            Children = new IToken<T>[1];
        }

        public override T ReturnValue()
        {
            return default(T);
        }
    }


    public class NumToken<T> : UnToken<T>, IFactorableToken<T>
    {
        new public T[] Children { get; set; }

        public NumToken()
        {
            Priority = int.MaxValue;
            Type = TokenType.Element;
            Children = new T[1];
        }

        public override T ReturnValue()
        {
            return Children[0];
        }

        public override string ToString()
        {
            return Children[0].ToString();
        }
    }

    public class ExpToken: UnToken<double>, IFactorableBracketsToken<double>
    {
        public IEnumerable<IFactorableToken<double>> BracketedTokens { get; set; }

        public ExpToken()
        {
            Priority = int.MaxValue;
            Type = TokenType.Function;
            Children = new IToken<double>[1];
        }

        public override double ReturnValue()
        {
            return System.Math.Exp(Children[0].ReturnValue());
        }

        public override string ToString()
        {
            return "exp(" + Children[0].ToString() + ")";
        }
    }



    public abstract class NToken<T> : IToken<T>, IFactorableToken<T>
    {
        virtual public int Priority { get; protected set; }
        virtual public TokenType Type { get; protected set; }

        public IToken<T>[] Children { get; set; }
        abstract public T ReturnValue();
    }

    public abstract class BinOpToken<T> : NToken<T>
    {
        public BinOpToken()
        {
            Type = TokenType.BinOperator;
            Children = new IToken<T>[2];
        }

        public override T ReturnValue()
        {
            return default(T);
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
            dynamic l = Children[0].ReturnValue();
            dynamic r = Children[1].ReturnValue();

            return l - r;
        }

        public override string ToString()
        {
            return Children[0].ToString() + " - " + Children[1].ToString();
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
            dynamic l = Children[0].ReturnValue();
            dynamic r = Children[1].ReturnValue();

            return l + r;
        }

        public override string ToString()
        {
            return Children[0].ToString() + " + " + Children[1].ToString();
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
            dynamic l = Children[0].ReturnValue();
            dynamic r = Children[1].ReturnValue();

            return l * r;
        }

        public override string ToString()
        {
            return Children[0].ToString() + " * " + Children[1].ToString();
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
            dynamic l = Children[0].ReturnValue();
            dynamic r = Children[1].ReturnValue();

            return l / r;
        }

        public override string ToString()
        {
            return Children[0].ToString() + " / " + Children[1].ToString();
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
            dynamic l = Children[0].ReturnValue();
            dynamic r = Children[1].ReturnValue();

            return l > r ? 1 : 0;
        }

        public override string ToString()
        {
            return Children[0].ToString() + " >" + Children[1].ToString();
        }
    }

    public class BracketToken<T> : UnToken<T>, IFactorableBracketsToken<T>
    {
        public IEnumerable<IFactorableToken<T>> BracketedTokens { get; set; }

        public BracketToken()
        {
            Priority = int.MaxValue;
            Type = TokenType.Brackets;
        }

        public override T ReturnValue()
        {
            return Children[0].ReturnValue();
        }

        public override string ToString()
        {
            return "(" + Children[0].ToString() + ")";
        }
    }

}
