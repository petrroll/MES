using MathExpressionSolver.Tokens;
using System.Collections.Generic;
using System;

namespace MathExpressionSolver.Parser
{
    class Tokenizer<T>
    {
        TokenFactory<T> _tokenFactory;
        public TokenFactory<T> TokenFactory { get { if (_tokenFactory == null) { _tokenFactory = new TokenFactory<T>(); } return _tokenFactory; }  set { _tokenFactory = value; } }

        private List<IFactorableToken<T>> tokens;
        private int currTokenIndex;

        private string[] parsedExpressions;
        private ParsedItemType[] parsedTypes;

        public IFactorableToken<T>[] Tokens { get { return tokens.ToArray(); } }

        public Dictionary<string, T> CustomVariables { set { TokenFactory.CustomVariables = value; } }

        public Tokenizer()
        {
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

            while (isCurrTokenIndexInRange())
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
            if (isAfterCurrTokenIndexSomething() && parsedTypes[currTokenIndex + 1] == ParsedItemType.LBracket)
            {
                return TokenFactory.CrateFunction(parsedExpressions[currTokenIndex], extractTokensFromFunctionArgs());
            }
            else
            {
                return TokenFactory.CreateVariable(parsedExpressions[currTokenIndex]);
            }
        }

        private IEnumerable<IFactorableToken<T>>[] extractTokensFromFunctionArgs()
        {
            if (isAfterCurrTokenIndexSomething())
            {
                currTokenIndex++;
                return extractTokensFromBrakets();
            }
            return null;
        }

        private IEnumerable<IFactorableToken<T>>[] extractTokensFromBrakets()
        {
            if(isAfterCurrTokenIndexSomething())
            {
                List<IFactorableToken<T>[]> arguements = new List<IFactorableToken<T>[]>();
                Tokenizer<T> arguementsTokenizer = new Tokenizer<T>() { TokenFactory = this.TokenFactory };

                int firstItemIndex = ++currTokenIndex;
                int length = 0;

                int bracketsLevel = 1;

                while (bracketsLevel > 0)
                {
                    if ((bracketsLevel == 1 &&
                        (parsedTypes[currTokenIndex] == ParsedItemType.RBracket ||
                        parsedTypes[currTokenIndex] == ParsedItemType.Separator)) ||
                        isCurrTokenIndexLast())
                    {
                        arguementsTokenizer.SetDataToBeTokenized(parsedExpressions.SubArray(firstItemIndex, length), parsedTypes.SubArray(firstItemIndex, length));
                        arguementsTokenizer.Tokenize();

                        arguements.Add(arguementsTokenizer.Tokens);

                        firstItemIndex = currTokenIndex + 1;
                        length = 0;

                        if(parsedTypes[currTokenIndex] == ParsedItemType.RBracket) { break; }
                    }
                    else
                    {
                        length++;
                    }


                    if (parsedTypes[currTokenIndex] == ParsedItemType.RBracket) { bracketsLevel--; }
                    else if (parsedTypes[currTokenIndex] == ParsedItemType.LBracket) { bracketsLevel++; }

                    currTokenIndex++;
                    if (isCurrTokenIndexOutOfRange()) { break; }
                }

                return arguements.ToArray();

            } else { return null; }

        }

        private bool isCurrTokenIndexInRange()
        {
            return (currTokenIndex < parsedExpressions.Length);
        }

        private bool isCurrTokenIndexOutOfRange()
        {
            return !(currTokenIndex < parsedExpressions.Length);
        }

        private bool isAfterCurrTokenIndexSomething()
        {
            return (parsedExpressions.Length - currTokenIndex > 1);
        }

        private bool isCurrTokenIndexLast()
        {
            return (parsedExpressions.Length - currTokenIndex == 1);
        }
    }

    public class TokenFactory<T>
    {
        public Dictionary<string, T> CustomVariables { get; set; }

        public IFactorableBracketsToken<T> CreateBrackets(IEnumerable<IFactorableToken<T>>[] arguements)
        {
            IFactorableBracketsToken<T> bracketToken = new BracketToken<T>();
            bracketToken.BracketedTokens[0] = arguements[0];
            return bracketToken;
        }

        public IFactorableBracketsToken<T> CrateFunction(string funcName, IEnumerable<IFactorableToken<T>>[] arguements)
        {
            IFactorableBracketsToken<T> bracketToken;
            switch (funcName)
            {
                case "exp":
                    bracketToken = (IFactorableBracketsToken<T>)new ExpToken();
                    bracketToken.BracketedTokens[0] = arguements[0];
                    return bracketToken;
                case "if":
                    bracketToken = (IFactorableBracketsToken<T>)new IfToken();
                    bracketToken.BracketedTokens[0] = arguements[0];
                    bracketToken.BracketedTokens[1] = arguements[1];
                    bracketToken.BracketedTokens[2] = arguements[2];
                    return bracketToken;
                default:
                    return null;
            }
        }

        public IFactorableToken<T> CreateNum(string s)
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
                default:
                    return null;
            }
        }
    }

}
