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

            while (isCurrTokenIndexInRange(currTokenIndex, parsedItems))
            {
                tokens.Add(getToken(ref currTokenIndex, parsedItems));
                currTokenIndex++;
            }

            return tokens.ToArray();
        }

        private IFactorableToken<T> getToken(ref int currTokenIndex, ParsedItem[] parsedItems)
        {
            switch (parsedItems[currTokenIndex].Type)
            {
                case ParsedItemType.Name:
                    return handleName(ref currTokenIndex, parsedItems);
                case ParsedItemType.Value:
                    return TokenFactory.CreateValue(parsedItems[currTokenIndex].Value);
                case ParsedItemType.LBracket:
                    return TokenFactory.CreateBrackets(extractTokensFromBrakets(ref currTokenIndex, parsedItems));
                case ParsedItemType.Operator:
                    return TokenFactory.CreateOperator(parsedItems[currTokenIndex].Value);
                case ParsedItemType.Invalid:
                    throw new TokenizerException(parsedItems[currTokenIndex].Value + " is not a valid expression.");
                default:
                    throw new TokenizerException(parsedItems[currTokenIndex].Value + " is not a valid expression.");
            }
        }

        private IFactorableToken<T> handleName(ref int currTokenIndex, ParsedItem[] parsedItems)
        {
            if (isSomethingAfterCurrTokenIndex(currTokenIndex, parsedItems) && parsedItems[currTokenIndex + 1].Type == ParsedItemType.LBracket)
            {
                return TokenFactory.CreateFunction(parsedItems[currTokenIndex].Value, extractTokensFromFunctionArgs(ref currTokenIndex, parsedItems));
            }
            else
            {
                return TokenFactory.CreateVariable(parsedItems[currTokenIndex].Value);
            }
        }

        private IFactorableToken<T>[][] extractTokensFromFunctionArgs(ref int currTokenIndex, ParsedItem[] parsedItems)
        {
            currTokenIndex++;
            return extractTokensFromBrakets(ref currTokenIndex, parsedItems);
        }

        private IFactorableToken<T>[][] extractTokensFromBrakets(ref int currTokenIndex, ParsedItem[] parsedItems)
        {
            if(isSomethingAfterCurrTokenIndex(currTokenIndex, parsedItems))
            {
                var arguments = new List<IFactorableToken<T>[]>();
                var argumentsTokenizer = new Tokenizer<T> { TokenFactory = this.TokenFactory };

                currTokenIndex++;

                int firstItemIndex = currTokenIndex;
                int length = 0;

                int bracketsLevel = 1;

                while (bracketsLevel != 0)
                {
                    if(isCurrTokenIndexOutOfRange(currTokenIndex, parsedItems))
                    {
                        throw new TokenizerException("Closing brackets for \"" + string.Join(string.Empty, parsedItems.SubArray(firstItemIndex - 1)) + "\" not found.");
                    }

                    if (parsedItems[currTokenIndex].Type == ParsedItemType.RBracket) { bracketsLevel--; }
                    else if (parsedItems[currTokenIndex].Type == ParsedItemType.LBracket) { bracketsLevel++; }

                    if (bracketsLevel == 1 && parsedItems[currTokenIndex].Type == ParsedItemType.Separator ||
                        (bracketsLevel == 0 && parsedItems[currTokenIndex].Type == ParsedItemType.RBracket && length > 0))
                    {
                        arguments.Add(returnTokenizedSubArray(argumentsTokenizer, firstItemIndex, length, parsedItems)); 

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

        private IFactorableToken<T>[] returnTokenizedSubArray(Tokenizer<T> argumentsTokenizer, int firstItemIndex, int length, ParsedItem[] parsedItems)
        {
            var subArrayParsedItems = parsedItems.SubArray(firstItemIndex, length);
            return argumentsTokenizer.Tokenize(subArrayParsedItems);
        }

        private bool isCurrTokenIndexInRange(int currTokenIndex, ParsedItem[] parsedItems)
        {
            return (currTokenIndex < parsedItems.Length);
        }

        private bool isCurrTokenIndexOutOfRange(int currTokenIndex, ParsedItem[] parsedItems)
        {
            return !isCurrTokenIndexInRange(currTokenIndex, parsedItems);
        }

        private bool isSomethingAfterCurrTokenIndex(int currTokenIndex, ParsedItem[] parsedItems)
        {
            return (isCurrTokenIndexInRange(currTokenIndex, parsedItems) && !isCurrTokenIndexLast(currTokenIndex, parsedItems));
        }

        private bool isCurrTokenIndexLast(int currTokenIndex, ParsedItem[] parsedItems)
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
