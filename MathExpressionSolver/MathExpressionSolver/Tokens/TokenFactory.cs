using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public interface ITokenFactory<T>
    {
        Dictionary<string, T> CustomVariables { get; set; }
        IFactorableBracketsToken<T> CreateBrackets(IFactorableToken<T>[][] arguments);
        IFactorableBracketsToken<T> CrateFunction(string s, IFactorableToken<T>[][] arguments);
        IFactorableToken<T> CreateValue(string s);
        IFactorableToken<T> CreateVariable(string s);
        IFactorableToken<T> CreateOperator(string s);

    }

    /// <summary>
    /// Handles <see cref="IFactorableToken{T}"/> creation out of <see cref="string"/> description.
    /// </summary>
    /// <typeparam name="T">Token base type.</typeparam>
    public abstract class TokenFactory<T> : ITokenFactory<T>
    {
        public Dictionary<string, T> CustomVariables { get; set; }

        public virtual IFactorableBracketsToken<T> CreateBrackets(IFactorableToken<T>[][] arguments)
        {
            IFactorableBracketsToken<T> bracketToken = new BracketToken<T>();
            bracketToken.BracketedTokens[0] = arguments[0];
            return bracketToken;
        }

        public virtual IFactorableBracketsToken<T> CrateFunction(string s, IFactorableToken<T>[][] arguments)
        {
            IFactorableBracketsToken<T> bracketToken;
            switch (s)
            {
                default:
                    throw new TokenizerException("No function named " + s + " exists.");
            }

            bracketToken.BracketedTokens = arguments;
            return bracketToken;
        }

        public abstract IFactorableToken<T> CreateValue(string s);

        public virtual IFactorableToken<T> CreateVariable(string s)
        {
            if(CustomVariables != null && CustomVariables.ContainsKey(s)) { return new ItemToken<T>() { Child = CustomVariables[s] }; }
            else { throw new TokenizerException("No variable named " + s + " exists."); }
        }

        public virtual IFactorableToken<T> CreateOperator(string s)
        {
            switch (s)
            {
                case "-":
                    return new MinusToken<T>();
                case "+":
                    return new PlusToken<T>();
                case "*":
                    return new TimesToken<T>();
                case "/":
                    return new DivToken<T>();
                default:
                    throw new TokenizerException("No operator named " + s + " exists.");
            }
        }
    }

    public class DoubleTokenFactory : TokenFactory<double>
    {
        public override IFactorableBracketsToken<double> CrateFunction(string s, IFactorableToken<double>[][] arguments)
        {
            IFactorableBracketsToken<double> bracketToken;
            switch (s)
            {
                case "exp":
                    bracketToken = new ExpToken();
                    break;
                case "if":
                    bracketToken = new IfToken();
                    break;
                case "ln":
                    bracketToken = new LnFunc();
                    break;
                case "sin":
                    bracketToken = new SinFunc();
                    break;
                case "cos":
                    bracketToken = new CosFunc();
                    break;
                case "tan":
                    bracketToken = new TanFunc();
                    break;
                case "sqrt":
                    bracketToken = new SqrtFunc();
                    break;
                default:
                    return base.CrateFunction(s, arguments);
            }
            bracketToken.BracketedTokens = arguments;
            return bracketToken;
        }

        public override IFactorableToken<double> CreateOperator(string s)
        {
            switch (s)
            {
                case ">":
                    return new GrtToken();
                case "<":
                    return new SmlrToken();
                default:
                    return base.CreateOperator(s);
            }
        }

        public override IFactorableToken<double> CreateValue(string s)
        {
            return new ItemToken<double>() { Child = double.Parse(s) };
        }
    }
}
