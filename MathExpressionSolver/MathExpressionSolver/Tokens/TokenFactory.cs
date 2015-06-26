using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public interface ITokenFactory<T>
    {
        Dictionary<string, T> CustomVariables { get; set; }
        IFactorableBracketsToken<T> CreateBrackets(IEnumerable<IFactorableToken<T>>[] arguments);
        IFactorableBracketsToken<T> CrateFunction(string funcName, IEnumerable<IFactorableToken<T>>[] arguments);
        IFactorableToken<T> CreateValue(string s);
        IFactorableToken<T> CreateVariable(string s);
        IFactorableToken<T> CreateOperator(string s);

    }

    public class TokenFactory<T> : ITokenFactory<T>
    {
        public Dictionary<string, T> CustomVariables { get; set; }

        public IFactorableBracketsToken<T> CreateBrackets(IEnumerable<IFactorableToken<T>>[] arguments)
        {
            IFactorableBracketsToken<T> bracketToken = new BracketToken<T>();
            bracketToken.BracketedTokens[0] = arguments[0];
            return bracketToken;
        }

        public IFactorableBracketsToken<T> CrateFunction(string funcName, IEnumerable<IFactorableToken<T>>[] arguments)
        {
            IFactorableBracketsToken<T> bracketToken;
            switch (funcName)
            {
                case "exp":
                    bracketToken = (IFactorableBracketsToken<T>)new ExpToken();
                    bracketToken.BracketedTokens[0] = arguments[0];
                    return bracketToken;
                case "if":
                    bracketToken = (IFactorableBracketsToken<T>)new IfToken();
                    bracketToken.BracketedTokens[0] = arguments[0];
                    bracketToken.BracketedTokens[1] = arguments[1];
                    bracketToken.BracketedTokens[2] = arguments[2];
                    return bracketToken;
                case "ln":
                    bracketToken = (IFactorableBracketsToken<T>)new LnFunc();
                    bracketToken.BracketedTokens[0] = arguments[0];
                    return bracketToken;
                case "sin":
                    bracketToken = (IFactorableBracketsToken<T>)new SinFunc();
                    bracketToken.BracketedTokens[0] = arguments[0];
                    return bracketToken;
                case "cos":
                    bracketToken = (IFactorableBracketsToken<T>)new CosFunc();
                    bracketToken.BracketedTokens[0] = arguments[0];
                    return bracketToken;
                case "tan":
                    bracketToken = (IFactorableBracketsToken<T>)new TanFunc();
                    bracketToken.BracketedTokens[0] = arguments[0];
                    return bracketToken;
                case "sqrt":
                    bracketToken = (IFactorableBracketsToken<T>)new SqrtFunc();
                    bracketToken.BracketedTokens[0] = arguments[0];
                    return bracketToken;

                default:
                    return null;
            }
        }

        public IFactorableToken<T> CreateValue(string s)
        {
            return (IFactorableToken<T>)new ItemToken<double>() { Child = double.Parse(s) };
        }

        public IFactorableToken<T> CreateVariable(string s)
        {
            return (CustomVariables != null && CustomVariables.ContainsKey(s)) ? new ItemToken<T>() { Child = CustomVariables[s] } : null;
        }

        public IFactorableToken<T> CreateOperator(string s)
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
                case ">":
                    return (IFactorableToken<T>)new GrtToken();
                case "<":
                    return (IFactorableToken<T>)new SmlrToken();
                default:
                    return null;
            }
        }
    }
}
