﻿using System;
using System.Collections.Generic;
using System.Globalization;
using MathExpressionSolver.Tokens;

namespace MathExpressionSolver.Builders
{
    public interface ITokenFactory<T>
    {
        Dictionary<string, T> CustomVariables { get; set; }
        IFactorableBracketsToken<T> CreateBrackets(IFactorableToken<T>[][] arguments);
        IFactorableBracketsToken<T> CreateFunction(string s, IFactorableToken<T>[][] arguments);
        IFactorableToken<T> CreateValue(string s);
        IFactorableToken<T> CreateVariable(string s);
        IFactorableToken<T> CreateOperator(string s);

    }

    public interface ICustomFunctionsAwareTokenFactory<T> : ITokenFactory<T>
    {
        string[] ArgsArray { set; }
        ICustFuncToken<T> CustomFunction { set; }
        Dictionary<string, IFactorableBracketsToken<T>> CustomFunctions { get; set; }

        void Clear();
    }

    public abstract class AdvancedTokenFactory<T> : ICustomFunctionsAwareTokenFactory<T>
    {
        private string[] argsArray;
        public string[] ArgsArray { set { if (value == null) { throw new ArgumentNullException(nameof(value), $"{nameof(ArgsArray)} doesn't accept null"); } argsArray = value; } }

        public ICustFuncToken<T> CustomFunction { private get; set; }
        public Dictionary<string, T> CustomVariables { get; set; }

        public Dictionary<string, IFactorableBracketsToken<T>> CustomFunctions { get; set; }

        protected AdvancedTokenFactory()
        {
            Clear();
        }

        public void Clear()
        {
            ArgsArray = new string[0];
            CustomFunction = null;
        }

        public virtual IFactorableBracketsToken<T> CreateBrackets(IFactorableToken<T>[][] arguments)
        {
            IFactorableBracketsToken<T> bracketToken = new BracketToken<T>();
            bracketToken.BracketedTokens[0] = arguments[0];
            return bracketToken;
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

        public virtual IFactorableToken<T> CreateVariable(string s)
        {
            int argID = Array.IndexOf(argsArray, s);
            if (argID != -1)
            {
                if (CustomFunction == null) { throw new InvalidOperationException("Custom function not set."); } 
                else if (!(CustomFunction.Children.Length > argID)) { throw new TokenizerException("Number of arguments for custom function don't match up."); }
                else { return new ArgToken<T> { ArgID = argID, CustFunction = CustomFunction }; }
            }
            else
            {
                if (CustomVariables != null && CustomVariables.ContainsKey(s)) { return new ItemToken<T> { Child = CustomVariables[s] }; }
                else { throw new TokenizerException("No variable named " + s + " exists."); }
            }
            
        }

        public virtual IFactorableBracketsToken<T> CreateFunction(string s, IFactorableToken<T>[][] arguments)
        {
            //TODO: Should create a new instance
            if(CustomFunctions != null && CustomFunctions.ContainsKey(s))
            {
                IFactorableBracketsToken<T> custFunc =  CustomFunctions[s];
                custFunc.BracketedTokens = arguments;
                return custFunc;
            }
            else { throw new TokenizerException("No function named " + s + " exists."); }
            
        }

        public abstract IFactorableToken<T> CreateValue(string s);
    }

    public class DoubleTokenFactory : AdvancedTokenFactory<double>
    {
        public override IFactorableBracketsToken<double> CreateFunction(string s, IFactorableToken<double>[][] arguments)
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
                    return base.CreateFunction(s, arguments);
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
            double result = 0;
            if(double.TryParse(s, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out result))
            {
                return new ItemToken<double> { Child = result };
            }
            else
            {
                throw new TokenizerException($"Number {s} is not in an appropriate format.");
            }

        }
    }
}
