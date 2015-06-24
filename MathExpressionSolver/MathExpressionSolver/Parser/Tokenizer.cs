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

        private ParsedItem[] parsedItems;

        public IFactorableToken<T>[] Tokens { get { return tokens.ToArray(); } }

        public Dictionary<string, T> CustomVariables { set { TokenFactory.CustomVariables = value; } }

        public Tokenizer()
        {
            tokens = new List<IFactorableToken<T>>();
            parsedItems = new ParsedItem[0];
        }

        public Tokenizer(ParsedItem[] parsedItems) : this()
        {
            SetDataToBeTokenized(parsedItems);
        }

        public void SetDataToBeTokenized(ParsedItem[] parsedItems)
        {
            Clear();

            this.parsedItems = parsedItems;
            tokens.Capacity = parsedItems.Length / 2;
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
            switch (parsedItems[currTokenIndex].Type)
            {
                case ParsedItemType.Name:
                    return handleName();
                case ParsedItemType.Element:
                    return TokenFactory.CreateNum(parsedItems[currTokenIndex].Value);
                case ParsedItemType.LBracket:
                    return TokenFactory.CreateBrackets(extractTokensFromBrakets());
                case ParsedItemType.Operator:
                    return TokenFactory.CreateOperator(parsedItems[currTokenIndex].Value);
                case ParsedItemType.Invalid:
                    return null;
                default:
                    return null;
            }
        }

        private IFactorableToken<T> handleName()
        {
            if (isAfterCurrTokenIndexSomething() && parsedItems[currTokenIndex + 1].Type == ParsedItemType.LBracket)
            {
                return TokenFactory.CrateFunction(parsedItems[currTokenIndex].Value, extractTokensFromFunctionArgs());
            }
            else
            {
                return TokenFactory.CreateVariable(parsedItems[currTokenIndex].Value);
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
                List<IFactorableToken<T>[]> arguments = new List<IFactorableToken<T>[]>();
                Tokenizer<T> argumentsTokenizer = new Tokenizer<T>() { TokenFactory = this.TokenFactory };

                int firstItemIndex = ++currTokenIndex;
                int length = 0;

                int bracketsLevel = 1;

                while (bracketsLevel > 0)
                {
                    if ((bracketsLevel == 1 && isCurrTypeArgumentEnding()) ||
                        isCurrTokenIndexLast())
                    {
                        arguments.Add(returnTokenizedSubArray(argumentsTokenizer, firstItemIndex, length));

                        firstItemIndex = currTokenIndex + 1;
                        length = 0;

                        if (parsedItems[currTokenIndex].Type == ParsedItemType.RBracket) { break; }
                    }
                    else
                    {
                        length++;
                    }


                    if (parsedItems[currTokenIndex].Type == ParsedItemType.RBracket) { bracketsLevel--; }
                    else if (parsedItems[currTokenIndex].Type == ParsedItemType.LBracket) { bracketsLevel++; }

                    currTokenIndex++;
                    if (isCurrTokenIndexOutOfRange()) { break; }
                }

                return arguments.ToArray();

            } else { return null; }

        }

        private IFactorableToken<T>[] returnTokenizedSubArray(Tokenizer<T> argumentsTokenizer, int firstItemIndex, int length)
        {
            argumentsTokenizer.SetDataToBeTokenized(parsedItems.SubArray(firstItemIndex, length));
            argumentsTokenizer.Tokenize();
            return argumentsTokenizer.Tokens;
        }

        private bool isCurrTypeArgumentEnding()
        {
            return (parsedItems[currTokenIndex].Type == ParsedItemType.RBracket ||
                     parsedItems[currTokenIndex].Type == ParsedItemType.Separator);
        }

        private bool isCurrTokenIndexInRange()
        {
            return (currTokenIndex < parsedItems.Length);
        }

        private bool isCurrTokenIndexOutOfRange()
        {
            return !(currTokenIndex < parsedItems.Length);
        }

        private bool isAfterCurrTokenIndexSomething()
        {
            return (parsedItems.Length - currTokenIndex > 1);
        }

        private bool isCurrTokenIndexLast()
        {
            return (parsedItems.Length - currTokenIndex == 1);
        }
    }

    public class TokenFactory<T>
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
                case "<":
                    return (IFactorableToken<T>)new SmlrToken();
                default:
                    return null;
            }
        }
    }

}
