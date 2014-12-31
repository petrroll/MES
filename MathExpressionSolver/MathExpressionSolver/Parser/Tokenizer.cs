using MathExpressionSolver.Tokens;
using System.Collections.Generic;

namespace MathExpressionSolver.Parser
{
    class Tokenizer
    {
        private List<IToken<double>> tokens;
        public IToken<double>[] Tokens { get { return tokens.ToArray(); } }

        public string[] parsedExpressions;
        public ParsedItemType[] parsedTypes;

        public Tokenizer()
        {
            tokens = new List<IToken<double>>();
        }

        public Tokenizer(string[] parsedExpressions, ParsedItemType[] parsedTypes) : this()
        {
            SetDataToBeTokenized(parsedExpressions, parsedTypes);
        }

        public void SetDataToBeTokenized(string[] parsedExpressions, ParsedItemType[] parsedTypes)
        {
            tokens.Clear();
            tokens.Capacity = parsedTypes.Length / 2;

            this.parsedExpressions = parsedExpressions;
            this.parsedTypes = parsedTypes;
        }

        public void Tokenize()
        {
            IToken<double> currToken;

            for (int i = 0; i < parsedExpressions.Length; i++)
            {
                currToken = getToken(i);
                if (currToken != null) { tokens.Add(currToken); }
            }
        }

        private IToken<double> getToken(int expIndex)
        {
            switch (parsedTypes[expIndex])
            {
                case ParsedItemType.Name:
                    break;
                case ParsedItemType.Element:
                    return TokenFactory.CreateNum(parsedExpressions[expIndex]);
                case ParsedItemType.LBracket:
                    break;
                case ParsedItemType.RBracket:
                    break;
                case ParsedItemType.Operator:
                    return TokenFactory.CreateOperator(parsedExpressions[expIndex]);
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
    }

    public static class TokenFactory
    {
        public static IToken<double> CreateNum(string s)
        {
            return new NumToken<double>() { Child = double.Parse(s) };
        }

        public static IToken<double> CreateOperator(string s)
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
