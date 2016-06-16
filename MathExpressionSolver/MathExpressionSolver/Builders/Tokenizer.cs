using System;
using System.Collections.Generic;
using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;
using System.Text;
using System.Diagnostics;

namespace MathExpressionSolver.Builders
{
    /// <summary>
    /// Creates linear array of <see cref="IFactorableToken{T}"/> out of <see cref="ParsedItem"/> array.
    /// </summary>
    /// <typeparam name="T">Token base type.</typeparam>
    public class Tokenizer<T>
    {
        /// <summary>
        /// <see cref="ITokenFactory{T}"/> object used for actual <see cref="IFactorableToken{T}"/> creation.
        /// </summary>
        public ITokenFactory<T> TokenFactory { get; set; }


        /// <summary>
        /// Tokenizes the <paramref name="parsedItems"/> and returns an array of <see cref="IFactorableToken{T}"/>.
        /// </summary>
        /// <param name="parsedItems">Array of <see cref="ParsedItem"/>s to be tokenized.</param>
        /// <exception cref="TokenizerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Tokenize"/> is called without set <see cref="TokenFactory"/>.</exception>
        /// <returns>Tokenized <paramref name="parsedItems"/></returns>
        public IFactorableToken<T>[] Tokenize(ParsedItem[] parsedItems)
        {
            if (parsedItems == null) { throw new ArgumentNullException(nameof(parsedItems), "ParsedItems null"); }
            if (TokenFactory == null) { throw new InvalidOperationException("Token factory not set."); }

            var tokenLevels = new Stack<TokenLevelInfo>();
            tokenLevels.Push(new TokenLevelInfo(new ParsedItem(string.Empty, ParsedItemType.NotSet)));

            int currTokenIndex = 0;
            var currentLayerTokens = tokenLevels.Peek();
            while (isInRange(parsedItems, currTokenIndex))
            {
                processCurrParsedItem(parsedItems, ref currTokenIndex, tokenLevels);
                currTokenIndex++;
            }

            if(tokenLevels.Count > 1) { throw new TokenizerException($"{tokenLevels.Peek()} does not have an ending parenthese."); }
            AssertHelper.AssertRuntime((tokenLevels.Count == 1), "Expression level toekinezer session was ended.");

            var topLevelTokens = tokenLevels.Peek().GetArguments();

            if (topLevelTokens.Length > 1) { throw new TokenizerException($"Top level contains multiple expressions: {tokenLevels.Peek()}."); }
            AssertHelper.AssertRuntime((topLevelTokens.Length == 1), "Expression level toekinezer session was ended.");

            return  topLevelTokens[0];
        }

        private void processCurrParsedItem(ParsedItem[] parsedItems, ref int currTokenIndex, Stack<TokenLevelInfo> tokenLevels)
        {
            var currItem = parsedItems[currTokenIndex];
            switch (currItem.Type)
            {
                case ParsedItemType.Name:
                    handleName(parsedItems, ref currTokenIndex, tokenLevels);
                    break;
                case ParsedItemType.Value:
                    handleValue(parsedItems, ref currTokenIndex, tokenLevels);
                    break;
                case ParsedItemType.LBracket:
                    handleLeftBracket(parsedItems, ref currTokenIndex, tokenLevels);
                    break;
                case ParsedItemType.RBracket:
                    handleRightBracket(parsedItems, ref currTokenIndex, tokenLevels);
                    break;
                case ParsedItemType.Operator:
                    handleOperator(parsedItems, ref currTokenIndex, tokenLevels);
                    break;
                case ParsedItemType.Separator:
                    handleSeparator(parsedItems, ref currTokenIndex, tokenLevels);
                    break;
                default:
                    throw new TokenizerException(parsedItems[currTokenIndex].Value + " is not a valid expression.");
            }
        }

#pragma warning disable RECS0154 // Parameter is never used
        private void handleSeparator(ParsedItem[] parsedItems, ref int currTokenIndex, Stack<TokenLevelInfo> tokenLevels)
#pragma warning restore RECS0154 // Parameter is never used
        {
            tokenLevels.Peek().SeparateNewArgument();
        }

        private void handleOperator(ParsedItem[] parsedItems, ref int currTokenIndex, Stack<TokenLevelInfo> tokenLevels)
        {
            var currParsedItem = parsedItems[currTokenIndex];
            var newToken = TokenFactory.CreateOperator(currParsedItem.Value);
            tokenLevels.Peek().AddNewToken(newToken);
        }

        private void handleName(ParsedItem[] parsedItems, ref int currTokenIndex, Stack<TokenLevelInfo> tokenLevels)
        {
            var currParsedItem = parsedItems[currTokenIndex];
            var isFunction = isNotEnd(parsedItems, currTokenIndex) && parsedItems[currTokenIndex + 1].Type == ParsedItemType.LBracket;

            if (isFunction)
            {
                tokenLevels.Push(new TokenLevelInfo(currParsedItem));
                currTokenIndex++;
            }
            else
            {
                var newToken = TokenFactory.CreateVariable(currParsedItem.Value);
                tokenLevels.Peek().AddNewToken(newToken);
            }
        }

        private void handleValue(ParsedItem[] parsedItems, ref int currTokenIndex, Stack<TokenLevelInfo> tokenLevels)
        {
            var currParsedItem = parsedItems[currTokenIndex];
            var newToken = TokenFactory.CreateValue(currParsedItem.Value);
            tokenLevels.Peek().AddNewToken(newToken);
        }

#pragma warning disable RECS0154 // Parameter is never used
        private void handleRightBracket(ParsedItem[] parsedItems, ref int currTokenIndex, Stack<TokenLevelInfo> tokenLevels)
#pragma warning restore RECS0154 // Parameter is never used
        {
            if (tokenLevels.Count < 2) { throw new TokenizerException($"No matching left bracket for {tokenLevels.Peek()}"); }

            var currLevelInfo = tokenLevels.Pop();
            var currParsedItem = currLevelInfo.ParsedItem;

            IFactorableToken<T> newToken = null;

            switch (currParsedItem.Type)
            {
                case ParsedItemType.Name:
                    newToken = TokenFactory.CreateFunction(currParsedItem.Value, currLevelInfo.GetArguments());
                    break;
                case ParsedItemType.LBracket:
                    newToken = TokenFactory.CreateBrackets(currLevelInfo.GetArguments());
                    break;
                default:
                    throw new InvalidOperationException("Invalid type initiated bracket.");
            }

            tokenLevels.Peek().AddNewToken(newToken);
        }

        private void handleLeftBracket(ParsedItem[] parsedItems, ref int currTokenIndex, Stack<TokenLevelInfo> tokenLevels)
        {
            var currParsedItem = parsedItems[currTokenIndex];
            tokenLevels.Push(new TokenLevelInfo(currParsedItem));
        }

        private enum TokenLevelChange { LevelDown, LevelUp, CurrLevel, ArgSeparation }
        struct CurrTokenInfo
        {
            public TokenLevelChange LevelChange { get; private set; }
            public IFactorableToken<T> Token { get; private set; }

            public CurrTokenInfo(TokenLevelChange levelChange, IFactorableToken<T> factorableToken)
            {
                this.LevelChange = levelChange;
                this.Token = factorableToken;
            }
        }

        class TokenLevelInfo
        {
            private readonly LinkedList<List<IFactorableToken<T>>> argumentsLists;
            public ParsedItem ParsedItem { get; private set; }

            public TokenLevelInfo(ParsedItem item)
            {
                ParsedItem = item;

                argumentsLists = new LinkedList<List<IFactorableToken<T>>>();
                argumentsLists.AddLast(new List<IFactorableToken<T>>());
            }

            public void SeparateNewArgument()
            {
                argumentsLists.AddLast(new List<IFactorableToken<T>>());
            }

            public void AddNewToken(IFactorableToken<T> token)
            {
                argumentsLists.Last.Value.Add(token);
            }

            public IFactorableToken<T>[][] GetArguments()
            {
                IFactorableToken<T>[][] arguments = new IFactorableToken<T>[argumentsLists.Count][];

                int i = 0;
                foreach (var argument in argumentsLists)
                {
                    arguments[i] = argument.ToArray();
                    i++;
                }

                return arguments;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                foreach (var argTokens in argumentsLists)
                {
                    foreach (var argToken in argTokens)
                    {
                        sb.Append(argToken);
                    }
                    sb.Append(";");
                }

                return sb.ToString();
            }
        }

        private IFactorableToken<T>[] returnTokenizedSubArray(ParsedItem[] parsedItems, int firstItemIndex, int length, Tokenizer<T> argumentsTokenizer)
        {
            var subArrayParsedItems = parsedItems.SubArray(firstItemIndex, length);
            return argumentsTokenizer.Tokenize(subArrayParsedItems);
        }

        private bool isInRange(ParsedItem[] parsedItems, int currTokenIndex)
        {
            return (currTokenIndex < parsedItems.Length);
        }

        private bool isOutOfRange(ParsedItem[] parsedItems, int currTokenIndex)
        {
            return !isInRange(parsedItems, currTokenIndex);
        }

        private bool isNotEnd(ParsedItem[] parsedItems, int currTokenIndex)
        {
            return (isInRange(parsedItems, currTokenIndex) && !isEnd(parsedItems, currTokenIndex));
        }

        private bool isEnd(ParsedItem[] parsedItems, int currTokenIndex)
        {
            return (parsedItems.Length - currTokenIndex == 1);
        }
    }

    [System.Serializable]
    public class TokenizerException : ExpressionException
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
