using MathExpressionSolver.Tokens;
using System.Collections.Generic;

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

            Clear();
        }

        public void Clear()
        {
            currTokenIndex = 0;

            tokens.Clear();
            tokens.Capacity = parsedTypes.Length / 2;
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
                    break;
                case ParsedItemType.Element:
                    return TokenFactory.CreateNum(parsedExpressions[currTokenIndex]);
                case ParsedItemType.LBracket:
                    return handleBrackets();
                case ParsedItemType.RBracket:
                    break;
                case ParsedItemType.Operator:
                    return TokenFactory.CreateOperator(parsedExpressions[currTokenIndex]);
                case ParsedItemType.Separator:
                    break;
                case ParsedItemType.WhiteSpace:
                    return null;
                case ParsedItemType.Invalid:
                    return null;
                default:
                    return null;


            }
            return null;
        }

        private IFactorableToken<double> handleBrackets()
        {
            List<int> a = new List<int>();
            if(parsedExpressions.Length - currTokenIndex > 1)
            {
                int firstItemIndex = currTokenIndex + 1;
                int leftBrackets = 1;
                while (leftBrackets > 0)
                {
                    currTokenIndex++;
                    if (currTokenIndex == parsedExpressions.Length) { break; }

                    if (parsedTypes[currTokenIndex] == ParsedItemType.RBracket) { leftBrackets--; }
                    else if (parsedTypes[currTokenIndex] == ParsedItemType.LBracket) { leftBrackets++; }
                }
                int lastItemIndex = currTokenIndex - 1;

                Tokenizer bracketedExpressionsTokenizer = new Tokenizer();
                bracketedExpressionsTokenizer.SetDataToBeTokenized(parsedExpressions.SubArray(firstItemIndex, lastItemIndex), parsedTypes.SubArray(firstItemIndex, lastItemIndex));
                bracketedExpressionsTokenizer.Tokenize();
                return new BracketToken<double>() { BracketedTokens = bracketedExpressionsTokenizer.Tokens };
            } else { return null; }

        }
    }

    public static class TokenFactory
    {
        public static IFactorableToken<double> CreateNum(string s)
        {
            return new NumToken<double>() { Child = double.Parse(s) };
        }

        public static IFactorableToken<double> CreateOperator(string s)
        {
            switch (s)
            {
                case "-":
                    return new MinusToken<double>();
                case "+":
                    return new PlusToken<double>();
                case "*":
                    return new TimesToken<double>();
                case "/":
                    return new DivToken<double>();
                default:
                    return null;
            }
        }
    }

}
