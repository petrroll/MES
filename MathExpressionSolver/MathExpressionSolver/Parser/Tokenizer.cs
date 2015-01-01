using MathExpressionSolver.Tokens;
using System.Collections.Generic;
using System;

namespace MathExpressionSolver.Parser
{
    class Tokenizer
    {
        private List<IFactorableToken<double>> tokens;
        private int currTokenIndex;
        public IFactorableToken<double>[] Tokens { get { return tokens.ToArray(); } }

        public string[] parsedExpressions;
        public ParsedItemType[] parsedTypes;

        public Tokenizer()
        {
            tokens = new List<IFactorableToken<double>>();
        }

        public Tokenizer(string[] parsedExpressions, ParsedItemType[] parsedTypes) : this()
        {
            SetDataToBeTokenized(parsedExpressions, parsedTypes);
        }

        public void SetDataToBeTokenized(string[] parsedExpressions, ParsedItemType[] parsedTypes)
        {
            this.parsedExpressions = parsedExpressions;
            this.parsedTypes = parsedTypes;

            tokens.Capacity = parsedTypes.Length / 2;

            Clear();
        }

        public void Clear()
        {
            currTokenIndex = 0;
            tokens.Clear();
        }

        public void Tokenize()
        {
            Clear();
            IFactorableToken<double> currToken;

            while (currTokenIndex < parsedExpressions.Length)
            {
                currToken = getToken();
                if (currToken != null) { tokens.Add(currToken); }
                currTokenIndex++;
            }
        }

        private IFactorableToken<double> getToken()
        {
            switch (parsedTypes[currTokenIndex])
            {
                case ParsedItemType.Name:
                    return TokenFactory.CrateFunction<double>(parsedExpressions[currTokenIndex], extractTokensFromFunctionArgs());
                case ParsedItemType.Element:
                    return TokenFactory.CreateNum(parsedExpressions[currTokenIndex]);
                case ParsedItemType.LBracket:
                    return TokenFactory.CreateBrackets(extractTokensFromBrakets());
                case ParsedItemType.Operator:
                    return TokenFactory.CreateOperator<double>(parsedExpressions[currTokenIndex]);
                case ParsedItemType.Invalid:
                    return null;
                default:
                    return null;
            }
        }

        private IEnumerable<IFactorableToken<double>> extractTokensFromFunctionArgs()
        {
            if (parsedExpressions.Length - currTokenIndex > 1)
            {
                currTokenIndex++;
                return extractTokensFromBrakets();
            }
            return null;
        }

        private IEnumerable<IFactorableToken<double>> extractTokensFromBrakets()
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

                Tokenizer bracketedExpressionsTokenizer = new Tokenizer();
                bracketedExpressionsTokenizer.SetDataToBeTokenized(parsedExpressions.SubArray(firstItemIndex, lastItemIndex), parsedTypes.SubArray(firstItemIndex, lastItemIndex));
                bracketedExpressionsTokenizer.Tokenize();

                return bracketedExpressionsTokenizer.Tokens;

            } else { return null; }

        }
    }

    public static class TokenFactory
    {
        public static IFactorableBracketsToken<T> CreateBrackets<T>(IEnumerable<IFactorableToken<T>> bracketedTokens)
        {
            return new BracketToken<T>() { BracketedTokens = bracketedTokens };
        }

        public static IFactorableBracketsToken<T> CrateFunction<T>(string funcName, IEnumerable<IFactorableToken<T>> bracketetTokens)
        {
            switch (funcName)
            {
                case "exp":
                    return (IFactorableBracketsToken<T>)new ExpToken() { BracketedTokens = (IEnumerable<IFactorableToken<double>>)bracketetTokens };
                default:
                    return null;
            }
        }

        public static IFactorableToken<double> CreateNum(string s)
        {
            return new NumToken<double>() { Child = double.Parse(s) };
        }

        public static IFactorableToken<T> CreateOperator<T>(string s)
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
                    return null;
            }
        }
    }

}
