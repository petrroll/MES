using MathExpressionSolver.Tokens;
using System.Collections.Generic;
using System;

namespace MathExpressionSolver.Parser
{
    class Tokenizer<T>
    {
        private List<IFactorableToken<T>> tokens;
        private int currTokenIndex;

        private string[] parsedExpressions;
        private ParsedItemType[] parsedTypes;

        public IFactorableToken<T>[] Tokens { get { return tokens.ToArray(); } }
        public TokenFactory<T> TokenFactory { get; set; }

        public Tokenizer()
        {
            TokenFactory = new TokenFactory<T>();
            tokens = new List<IFactorableToken<T>>();

            parsedExpressions = new string[0];
            parsedTypes = new ParsedItemType[0];
        }

        public Tokenizer(string[] parsedExpressions, ParsedItemType[] parsedTypes) : this()
        {
            SetDataToBeTokenized(parsedExpressions, parsedTypes);
        }

        public void SetDataToBeTokenized(string[] parsedExpressions, ParsedItemType[] parsedTypes)
        {
            this.parsedExpressions = parsedExpressions;
            this.parsedTypes = parsedTypes;

            Clear();

            tokens.Capacity = parsedTypes.Length / 2;
        }

        public void Clear()
        {
            currTokenIndex = 0;
            tokens.Clear();
        }

        public void Tokenize()
        {
            Clear();
            IFactorableToken<T> currToken;

            while (currTokenIndex < parsedExpressions.Length)
            {
                currToken = getToken();
                if (currToken != null) { tokens.Add(currToken); }
                currTokenIndex++;
            }
        }

        private IFactorableToken<T> getToken()
        {
            switch (parsedTypes[currTokenIndex])
            {
                case ParsedItemType.Name:
                    return handleName();
                case ParsedItemType.Element:
                    return TokenFactory.CreateNum(parsedExpressions[currTokenIndex]);
                case ParsedItemType.LBracket:
                    return TokenFactory.CreateBrackets(extractTokensFromBrakets());
                case ParsedItemType.Operator:
                    return TokenFactory.CreateOperator(parsedExpressions[currTokenIndex]);
                case ParsedItemType.Invalid:
                    return null;
                default:
                    return null;
            }
        }

        private IFactorableToken<T> handleName()
        {
            if (parsedExpressions.Length - currTokenIndex > 1 && parsedTypes[currTokenIndex + 1] == ParsedItemType.LBracket)
            {
                return TokenFactory.CrateFunction(parsedExpressions[currTokenIndex], extractTokensFromFunctionArgs());
            }
            else
            {
                return TokenFactory.CreateVariable(parsedExpressions[currTokenIndex]);
            }
        }

        private IEnumerable<IFactorableToken<T>> extractTokensFromFunctionArgs()
        {
            if (parsedExpressions.Length - currTokenIndex > 1)
            {
                currTokenIndex++;
                return extractTokensFromBrakets();
            }
            return null;
        }

        private IEnumerable<IFactorableToken<T>> extractTokensFromBrakets()
        {
            List<int> a = new List<int>();
            if(parsedExpressions.Length - currTokenIndex > 1)
            {
                int firstItemIndex = currTokenIndex + 1;
                int bracketsLevel = 1;
                while (bracketsLevel > 0)
                {
                    currTokenIndex++;
                    if (currTokenIndex == parsedExpressions.Length) { break; }

                    if (parsedTypes[currTokenIndex] == ParsedItemType.RBracket) { bracketsLevel--; }
                    else if (parsedTypes[currTokenIndex] == ParsedItemType.LBracket) { bracketsLevel++; }
                }
                int lastItemIndex = currTokenIndex - 1;

                Tokenizer<T> bracketedExpressionsTokenizer = new Tokenizer<T>() { TokenFactory = this.TokenFactory };
                bracketedExpressionsTokenizer.SetDataToBeTokenized(parsedExpressions.SubArray(firstItemIndex, lastItemIndex), parsedTypes.SubArray(firstItemIndex, lastItemIndex));
                bracketedExpressionsTokenizer.Tokenize();

                return bracketedExpressionsTokenizer.Tokens;

            } else { return null; }

        }
    }

    public class TokenFactory<T>
    {
        public Dictionary<string, T> CustomVariables { get; set; }

        public IFactorableBracketsToken<T> CreateBrackets(IEnumerable<IFactorableToken<T>> bracketedTokens)
        {
            return new BracketToken<T>() { BracketedTokens = bracketedTokens };
        }

        public IFactorableBracketsToken<T> CrateFunction(string funcName, IEnumerable<IFactorableToken<T>> bracketetTokens)
        {
            switch (funcName)
            {
                case "exp":
                    return (IFactorableBracketsToken<T>)new ExpToken() { BracketedTokens = (IEnumerable<IFactorableToken<double>>)bracketetTokens };
                default:
                    return null;
            }
        }

        public IFactorableToken<T> CreateNum(string s)
        {
            return (IFactorableToken<T>)new NumToken<double>() { Child = double.Parse(s) };
        }

        public IFactorableToken<T> CreateVariable(string s)
        {
            return (CustomVariables != null && CustomVariables.ContainsKey(s)) ? new NumToken<T>() { Child = CustomVariables[s] } : null; 
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
                default:
                    return null;
            }
        }
    }

}
