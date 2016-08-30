using System;
using System.Collections;
using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public enum TokenType { BinOperator, Function, Element, Brackets, ArgToken };

    public abstract class Token<T> : IFactorableToken<T>
    {
        virtual public int Priority { get; protected set; }
        virtual public TokenType Type { get; protected set; }

        public abstract IToken<T> Clone(IDictionary<IToken<T>, IToken<T>> substitutionDict);
        abstract public T ReturnValue();

        protected IToken<T> CloneIfNotInSubstitionDict(IToken<T> originalPosition, IDictionary<IToken<T>, IToken<T>> substitutionDict)
        {
            if (originalPosition == null) { return null; }
            if (substitutionDict.ContainsKey(originalPosition))
            { return substitutionDict[originalPosition]; }
            else { return originalPosition.Clone(substitutionDict); }
        }

    }

    public abstract class UnToken<T> : Token<T>, IFactorableUnToken<T>
    {
        public IToken<T> Child { get { return MutChild; } }
        public IToken<T> MutChild { get; set; }

        protected UnToken()
        {
            Priority = int.MaxValue;
            Type = TokenType.ArgToken;
        }

        public override T ReturnValue()
        {
            return Child.ReturnValue();
        }

        public override IToken<T> Clone(IDictionary<IToken<T>, IToken<T>> substitutionDict)
        {
            var clone = (UnToken<T>)MemberwiseClone();
            clone.MutChild = CloneIfNotInSubstitionDict(Child, substitutionDict);
     
            return clone;

        }
    }

    public class ItemToken<T> : Token<T>
    {
        public T Value { get; set; }

        public ItemToken()
        {
            Type = TokenType.Element;
            Priority = int.MaxValue;
        }

        public override T ReturnValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override IToken<T> Clone(IDictionary<IToken<T>, IToken<T>> substitutionDict)
        {
            return (ItemToken<T>)MemberwiseClone();
        }
    }

    public class ArgToken<T> : UnToken<T>
    {
    }

    public abstract class BinOpToken<T> : Token<T>, IFactorableBinToken<T>
    {
        public IToken<T> LeftChild { get { return MutLeftChild; } }
        public IToken<T> RightChild { get { return MutRightChild; } }

        public IToken<T> MutLeftChild { get; set; }
        public IToken<T> MutRightChild { get; set; }

        protected BinOpToken()
        {
            Type = TokenType.BinOperator;
        }

        public override T ReturnValue()
        {
            return default(T);
        }

        public override IToken<T> Clone(IDictionary<IToken<T>, IToken<T>> substitutionDict)
        {
            var copy = (BinOpToken<T>)this.MemberwiseClone();

            copy.MutLeftChild = CloneIfNotInSubstitionDict(LeftChild, substitutionDict);
            copy.MutRightChild = CloneIfNotInSubstitionDict(RightChild, substitutionDict);

            return copy;
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
        public IReadOnlyList<IToken<T>> Children { get; protected set; }

        private IToken<T>[] children { get; set; }

        protected FuncToken(int arguments)
        {
            children = new IToken<T>[arguments];
            Children = children;

            BracketedTokens = new IFactorableToken<T>[arguments][];

            Type = TokenType.Function;
            Priority = int.MaxValue;
        }

        public override T ReturnValue()
        {
            return default(T);
        }

        public override string ToString()
        {
            return "(" + default(T) + ")";
        }

        public override IToken<T> Clone(IDictionary<IToken<T>, IToken<T>> substitutionDict)
        {
            var argCount = Children.Count;
            IToken<T>[] copiedChildren = new IToken<T>[argCount];

            for (int i = 0; i < argCount; i++)
            {
                var currChild = Children[i];
                copiedChildren[i] = CloneIfNotInSubstitionDict(Children[i], substitutionDict);
            }

            var copy = (FuncToken<T>)this.MemberwiseClone();

            copy.children = copiedChildren;
            copy.Children = copiedChildren;

            copy.BracketedTokens = new IFactorableToken<T>[argCount][];

            return copy;
        }

        public void SetChild(int index, IToken<T> child)
        {
            children[index] = child;
        }
    }

    public class CustFuncToken<T> : Token<T>, IFactorableCustFuncToken<T>
    {
        public IFactorableToken<T>[][] BracketedTokens { get; set; }

        public IToken<T> FuncTopToken { get { return MutFuncTopToken; }  }
        public IToken<T> MutFuncTopToken { get; set; }

        public IFactorableUnToken<T>[] MutArgumentTokens { get; set; }

        public IReadOnlyList<IToken<T>> Children { get { return new ReadOnlyChildrenProxy(MutArgumentTokens); } }

        public CustFuncToken(int argNum)
        {
            Type = TokenType.Function;
            Priority = int.MaxValue;

            MutArgumentTokens = new IFactorableUnToken<T>[argNum];
        }

        public override T ReturnValue()
        {
            return FuncTopToken.ReturnValue();
        }

        public void SetChild(int index, IToken<T> child)
        {
            if(MutArgumentTokens[index] == null) { return; }
            MutArgumentTokens[index].MutChild = child;
        }

        public override IToken<T> Clone(IDictionary<IToken<T>, IToken<T>> substitutionDict)
        {
            var clone = (CustFuncToken<T>)MemberwiseClone();
            var mutArgumentsCopy = new IFactorableUnToken<T>[MutArgumentTokens.Length];

            for (int i = 0; i < MutArgumentTokens.Length; i++)
            {
                var currArg = MutArgumentTokens[i];

                if (currArg != null)
                {
                    var currArgCopy = currArg.Clone(substitutionDict);
                    substitutionDict[currArg] = currArgCopy;

                    mutArgumentsCopy[i] = (IFactorableUnToken<T>)currArgCopy;
                }

            }


            clone.MutArgumentTokens = mutArgumentsCopy;
            clone.MutFuncTopToken = CloneIfNotInSubstitionDict(FuncTopToken, substitutionDict);

            return clone;
        }
         
        struct ReadOnlyChildrenProxy : IReadOnlyList<IToken<T>>
        {
            private IFactorableUnToken<T>[] ArgumentTokens {get; set;}

            public ReadOnlyChildrenProxy(IFactorableUnToken<T>[] args)
            {
                ArgumentTokens = args;
            }

            public IToken<T> this[int index] { get { return ArgumentTokens[index].Child; } }
            public int Count => ArgumentTokens.Length; 

            public IEnumerator<IToken<T>> GetEnumerator()
            {
                foreach (var item in ArgumentTokens)
                {
                    yield return item?.Child;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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
