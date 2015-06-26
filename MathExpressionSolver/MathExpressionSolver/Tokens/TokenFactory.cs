﻿using System.Collections.Generic;

namespace MathExpressionSolver.Tokens
{
    public interface ITokenFactory<T>
    {
        Dictionary<string, T> CustomVariables { get; set; }
        IFactorableBracketsToken<T> CreateBrackets(IFactorableToken<T>[][] arguments);
        IFactorableBracketsToken<T> CrateFunction(string funcName, IFactorableToken<T>[][] arguments);
        IFactorableToken<T> CreateValue(string s);
        IFactorableToken<T> CreateVariable(string s);
        IFactorableToken<T> CreateOperator(string s);

    }

    public class TokenFactory<T> : ITokenFactory<T>
    {
        public Dictionary<string, T> CustomVariables { get; set; }

        public virtual IFactorableBracketsToken<T> CreateBrackets(IFactorableToken<T>[][] arguments)
        {
            IFactorableBracketsToken<T> bracketToken = new BracketToken<T>();
            bracketToken.BracketedTokens[0] = arguments[0];
            return bracketToken;
        }

        public virtual IFactorableBracketsToken<T> CrateFunction(string funcName, IFactorableToken<T>[][] arguments)
        {
            IFactorableBracketsToken<T> bracketToken;
            switch (funcName)
            {
                case "exp":
                    bracketToken = (IFactorableBracketsToken<T>)new ExpToken();
                    break;
                case "if":
                    bracketToken = (IFactorableBracketsToken<T>)new IfToken();
                    break;
                case "ln":
                    bracketToken = (IFactorableBracketsToken<T>)new LnFunc();
                    break;
                case "sin":
                    bracketToken = (IFactorableBracketsToken<T>)new SinFunc();
                    break;
                case "cos":
                    bracketToken = (IFactorableBracketsToken<T>)new CosFunc();
                    break;
                case "tan":
                    bracketToken = (IFactorableBracketsToken<T>)new TanFunc();
                    break;
                case "sqrt":
                    bracketToken = (IFactorableBracketsToken<T>)new SqrtFunc();
                    break;
                default:
                    return null;
            }
            bracketToken.BracketedTokens = arguments;
            return bracketToken;
        }

        public virtual IFactorableToken<T> CreateValue(string s)
        {
            return (IFactorableToken<T>)new ItemToken<double>() { Child = double.Parse(s) };
        }

        public virtual IFactorableToken<T> CreateVariable(string s)
        {
            return (CustomVariables != null && CustomVariables.ContainsKey(s)) ? new ItemToken<T>() { Child = CustomVariables[s] } : null;
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
