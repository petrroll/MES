using System;
using System.Collections.Generic;
using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;

namespace MathExpressionSolver.Builders
{
    /// <summary>
    /// Creates linear array of <see cref="IFactorableToken<T>"/> out of <see cref="ParsedItem"/> array.
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

            var tokens = new List<IFactorableToken<T>>();
            int currTokenIndex = 0;

            while (isInRange(parsedItems, currTokenIndex))
            {
                tokens.Add(getToken(parsedItems, ref currTokenIndex));
                currTokenIndex++;
            }

            return tokens.ToArray();
        }

        private IFactorableToken<T> getToken(ParsedItem[] parsedItems, ref int currTokenIndex)
        {
            switch (parsedItems[currTokenIndex].Type)
            {
                case ParsedItemType.Name:
                    return handleName(parsedItems, ref currTokenIndex);
                case ParsedItemType.Value:
                    return TokenFactory.CreateValue(parsedItems[currTokenIndex].Value);
                case ParsedItemType.LBracket:
                    return TokenFactory.CreateBrackets(extractTokensFromBrakets(parsedItems, ref currTokenIndex));
                case ParsedItemType.Operator:
                    return TokenFactory.CreateOperator(parsedItems[currTokenIndex].Value);
                case ParsedItemType.Invalid:
                    throw new TokenizerException(parsedItems[currTokenIndex].Value + " is not a valid expression.");
                default:
                    throw new TokenizerException(parsedItems[currTokenIndex].Value + " is not a valid expression.");
            }
        }

        private IFactorableToken<T> handleName(ParsedItem[] parsedItems, ref int currTokenIndex)
        {
            var name = parsedItems[currTokenIndex].Value;
            if (isNotEnd(parsedItems, currTokenIndex) && parsedItems[currTokenIndex + 1].Type == ParsedItemType.LBracket)
            {
                currTokenIndex++;
                var argsTokens = extractTokensFromBrakets(parsedItems, ref currTokenIndex);
                return TokenFactory.CreateFunction(name, argsTokens);
            }
            else
            {
                return TokenFactory.CreateVariable(name);
            }
        }

        private IFactorableToken<T>[][] extractTokensFromBrakets(ParsedItem[] parsedItems, ref int currTokenIndex)
        {
            if (isEnd(parsedItems, currTokenIndex))
            { throw new TokenizerException("Unended bracket at the end of the expression."); }

            var arguments = new List<IFactorableToken<T>[]>();
            var argumentsTokenizer = new Tokenizer<T> { TokenFactory = this.TokenFactory };

            currTokenIndex++;

            int firstItemIndex = currTokenIndex;
            int length = 0;

            int bracketsLevel = 1;

            while (bracketsLevel != 0)
            {
                if (isOutOfRange(parsedItems, currTokenIndex))
                {
                    throw new TokenizerException("Closing brackets for \"" + string.Join(string.Empty, parsedItems.SubArray(firstItemIndex - 1)) + "\" not found.");
                }

                if (parsedItems[currTokenIndex].Type == ParsedItemType.RBracket) { bracketsLevel--; }
                else if (parsedItems[currTokenIndex].Type == ParsedItemType.LBracket) { bracketsLevel++; }

                if (bracketsLevel == 1 && parsedItems[currTokenIndex].Type == ParsedItemType.Separator ||
                    (bracketsLevel == 0 && parsedItems[currTokenIndex].Type == ParsedItemType.RBracket && length > 0))
                {
                    arguments.Add(returnTokenizedSubArray(parsedItems, firstItemIndex, length, argumentsTokenizer));

                    firstItemIndex = currTokenIndex + 1;
                    length = 0;
                }
                else { length++; }

                currTokenIndex++;

            }

            currTokenIndex--; //corrects index to last brackets token.
            return arguments.ToArray();
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
