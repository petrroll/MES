using MathExpressionSolver.Tokens;
using System.Collections.Generic;

namespace MathExpressionSolver.Parser
{
    class Tokenizer<T>
    {
        TokenFactory<T> _tokenFactory;
        private TokenFactory<T> tokenFactory { get { if (_tokenFactory == null) { _tokenFactory = new TokenFactory<T>(); } return _tokenFactory; }  set { _tokenFactory = value; } }

        private List<IFactorableToken<T>> tokens;
        private int currTokenIndex;

        private ParsedItem[] parsedItems;
        public ParsedItem[] DataToBeTokenized
        {
            set
            {
                Clear();

                this.parsedItems = value;
                tokens.Capacity = parsedItems.Length / 2;
            }
        }

        public IFactorableToken<T>[] Tokens { get { return tokens.ToArray(); } }
        public Dictionary<string, T> CustomVariables { set { tokenFactory.CustomVariables = value; } }

        public Tokenizer()
        {
            tokens = new List<IFactorableToken<T>>();
            parsedItems = new ParsedItem[0];
        }

        public Tokenizer(ParsedItem[] parsedItems) : this()
        {
            DataToBeTokenized = parsedItems;
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
                    return tokenFactory.CreateNum(parsedItems[currTokenIndex].Value);
                case ParsedItemType.LBracket:
                    return tokenFactory.CreateBrackets(extractTokensFromBrakets());
                case ParsedItemType.Operator:
                    return tokenFactory.CreateOperator(parsedItems[currTokenIndex].Value);
                case ParsedItemType.Invalid:
                    throw new TokenizerException(parsedItems[currTokenIndex].Value + " is not a valid expression.");
                default:
                    throw new TokenizerException(parsedItems[currTokenIndex].Value + " is not a valid expression.");
            }
        }

        private IFactorableToken<T> handleName()
        {
            if (isSomethingAfterCurrTokenIndex() && parsedItems[currTokenIndex + 1].Type == ParsedItemType.LBracket)
            {
                return tokenFactory.CrateFunction(parsedItems[currTokenIndex].Value, extractTokensFromFunctionArgs());
            }
            else
            {
                return tokenFactory.CreateVariable(parsedItems[currTokenIndex].Value);
            }
        }

        private IEnumerable<IFactorableToken<T>>[] extractTokensFromFunctionArgs()
        {
            currTokenIndex++;
            return extractTokensFromBrakets();
        }

        private IEnumerable<IFactorableToken<T>>[] extractTokensFromBrakets()
        {
            if(isSomethingAfterCurrTokenIndex())
            {
                List<IFactorableToken<T>[]> arguments = new List<IFactorableToken<T>[]>();
                Tokenizer<T> argumentsTokenizer = new Tokenizer<T>() { tokenFactory = this.tokenFactory };

                currTokenIndex++;

                int firstItemIndex = currTokenIndex;
                int length = 0;

                int bracketsLevel = 1;

                while (bracketsLevel != 0)
                {
                    if(isCurrTokenIndexOutOfRange())
                    {
                        throw new TokenizerException("Closing brackets for \"" + string.Join(string.Empty, parsedItems.SubArray(firstItemIndex - 1)) + "\" not found.");
                    }

                    if (parsedItems[currTokenIndex].Type == ParsedItemType.RBracket) { bracketsLevel--; }
                    else if (parsedItems[currTokenIndex].Type == ParsedItemType.LBracket) { bracketsLevel++; }

                    if (bracketsLevel == 1 && parsedItems[currTokenIndex].Type == ParsedItemType.Separator ||
                        bracketsLevel == 0 && parsedItems[currTokenIndex].Type == ParsedItemType.RBracket)
                    {
                        arguments.Add(returnTokenizedSubArray(argumentsTokenizer, firstItemIndex, length));

                        firstItemIndex = currTokenIndex + 1;
                        length = 0;
                    }
                    else { length++; }

                    currTokenIndex++;
                 
                }

                currTokenIndex--; //corrects index to last brackets token.
                return arguments.ToArray();

            } else { throw new TokenizerException("Unended bracket at the end of the expression."); }

        }

        private IFactorableToken<T>[] returnTokenizedSubArray(Tokenizer<T> argumentsTokenizer, int firstItemIndex, int length)
        {
            argumentsTokenizer.DataToBeTokenized = parsedItems.SubArray(firstItemIndex, length);
            argumentsTokenizer.Tokenize();
            return argumentsTokenizer.Tokens;
        }

        private bool isCurrTokenIndexInRange()
        {
            return (currTokenIndex < parsedItems.Length);
        }

        private bool isCurrTokenIndexOutOfRange()
        {
            return !isCurrTokenIndexInRange();
        }

        private bool isSomethingAfterCurrTokenIndex()
        {
            return (isCurrTokenIndexInRange() && !isCurrTokenIndexLast());
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


    [System.Serializable]
    public class TokenizerException : System.Exception
    {
        public TokenizerException() { }
        public TokenizerException(string message) : base(message) { }
        public TokenizerException(string message, System.Exception inner) : base(message, inner) { }
        protected TokenizerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }

}
