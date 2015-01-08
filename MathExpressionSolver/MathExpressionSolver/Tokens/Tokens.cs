using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public enum TokenType { BinOperator, Function, Element, Brackets };

    public abstract class Token<T> : IToken<T>, IFactorableToken<T>
    {
        virtual public int Priority { get; protected set; }
        virtual public TokenType Type { get; protected set; }

        public IToken<T>[] Children { get; protected set; }
        abstract public T ReturnValue();
    }

    public abstract class UnToken<T> : Token<T>, IUnToken<T>
    {
        public virtual IToken<T> Child { get { return Children[0]; } set { Children[0] = value; } }

        public UnToken() : base()
        {
            Priority = int.MaxValue;
            Children = new IToken<T>[1];
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

        public ItemToken() : base()
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

    public abstract class BinOpToken<T> : Token<T>, IBinToken<T>
    {
        public IToken<T> LeftChild { get { return Children[0]; } set { Children[0] = value; } }
        public IToken<T> RightChild { get { return Children[1]; } set { Children[1] = value; } }

        public BinOpToken() : base()
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

    public class SmlrToken : BinOpToken<double>
    {
        public SmlrToken() : base()
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
            return LeftChild.ToString() + " >" + RightChild.ToString();
        }
    }

    public abstract class FuncToken<T> : Token<T>, IFactorableBracketsToken<T>
    {
        public IEnumerable<IFactorableToken<T>>[] BracketedTokens { get; set; }

        public FuncToken(int arguments) : base()
        {
            Type = TokenType.Function;
            Priority = int.MaxValue;

            Children = new IToken<T>[arguments];
            BracketedTokens = new IEnumerable<IFactorableToken<T>>[arguments];
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
            return "(" + Children[0].ToString() + ")";
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
            return "exp(" + Children[0].ToString() + ")";
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
            return "ln(" + Children[0].ToString() + ")";
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
            return "sqrt(" + Children[0].ToString() + ")";
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
            return "sin(" + Children[0].ToString() + ")";
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
            return "cos(" + Children[0].ToString() + ")";
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
            return "tan(" + Children[0].ToString() + ")";
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
            return (Children[0].ReturnValue() != 0) ? Children[1].ReturnValue() : Children[2].ReturnValue();
        }

        public override string ToString()
        {
            return "if(" + Children[0].ToString() + ";" + Children[1].ToString() + ";" + Children[2].ToString() + ")";
        }
    }

}
