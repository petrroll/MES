using System;
using System.Collections.Generic;
using System.Globalization;
using MathExpressionSolver.Tokens;
using MathExpressionSolver.Interfaces;

namespace MathExpressionSolver.Builders
{
    public abstract class AdvancedTokenFactory<T> : ICustomFunctionsAwareTokenFactory<T>
    {
        private string[] argsArray;
        public string[] ArgsArray { set { if (value == null) { throw new ArgumentNullException(nameof(value), $"{nameof(ArgsArray)} doesn't accept null"); } argsArray = value; } }

        public IFactorableCustFuncToken<T> CustomFunction { private get; set; }
        public Dictionary<string, T> CustomVariables { get; set; }

        public Dictionary<string, IFactorableCustFuncToken<T>> CustomFunctions { get; set; }

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
                    throw new TokenizerException($"No operator named {s} exists.");
            }
        }

        public virtual IFactorableToken<T> CreateVariable(string s)
        {
            int argID = Array.IndexOf(argsArray, s);
            if (argID != -1)
            {
                if (CustomFunction == null) { throw new InvalidOperationException("Custom function not set."); } 
                else if (!(CustomFunction.Children.Count > argID)) { throw new TokenizerException("Number of arguments for custom function don't match up."); }
                else
                {
                    if(CustomFunction.MutArgumentTokens[argID] == null)
                    {
                        var newArgToken = new ArgToken<T>();
                        CustomFunction.MutArgumentTokens[argID] = newArgToken;
                        return newArgToken;
                    }

                    return CustomFunction.MutArgumentTokens[argID];

                }
            }
            else
            {
                if (CustomVariables != null && CustomVariables.ContainsKey(s)) { return new ItemToken<T> { Value = CustomVariables[s] }; }
                else { throw new TokenizerException($"No variable named {s} exists."); }
            }
            
        }

        public virtual IFactorableBracketsToken<T> CreateFunction(string s, IFactorableToken<T>[][] arguments)
        {
            if(CustomFunctions != null && CustomFunctions.ContainsKey(s))
            {
                var custFunc = (IFactorableBracketsToken<T>)CustomFunctions[s].Clone(new Dictionary<IToken<T>, IToken<T>>());
                custFunc.BracketedTokens = arguments;
                return custFunc;
            }
            else { throw new TokenizerException($"No function named {s} exists."); }
            
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
                return new ItemToken<double> { Value = result };
            }
            else
            {
                throw new TokenizerException($"Number {s} is not in an appropriate format.");
            }

        }
    }
}
