using MathExpressionSolver.Tokens;
using System.Collections.Generic;

namespace MathExpressionSolver.Parser
{
    class Tokenizer
    {
        private List<IToken<int>> tokens;
        public IToken<int>[] Tokens { get { return tokens.ToArray(); } }

        public string[] parsedExpressions;
        public ParsedItemType[] parsedTypes;

        public Tokenizer()
        {
            tokens = new List<IToken<int>>();
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
            IToken<int> currToken;

            for (int i = 0; i < parsedExpressions.Length; i++)
            {
                currToken = getToken(i);
                if (currToken != null) { tokens.Add(currToken); }
            }
        }

        private IToken<int> getToken(int expIndex)
        {
            switch (parsedTypes[expIndex])
            {
                case ParsedItemType.Name:
                    break;
                case ParsedItemType.Element:
                    return TokenFactory.CreateInt(parsedExpressions[expIndex]);
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
        public static IToken<int> CreateInt(string s)
        {
            return new IntToken() { Child = int.Parse(s) };
        }

        public static IToken<int> CreateOperator(string s)
        {
            switch (s)
            {
                case "-":
                    return new MinusToken();
                case "+":
                    return new PlusToken();
                case "*":
                    return new TimesToken();
                case "/":
                    return new DivToken();
                default:
                    return null;
            }
        }
    }
}
