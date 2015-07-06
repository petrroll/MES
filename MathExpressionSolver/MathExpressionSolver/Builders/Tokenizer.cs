using System;
using System.Collections.Generic;
using MathExpressionSolver.Parser;

namespace MathExpressionSolver.Tokens
{
    /// <summary>
    /// Creates linear array of <see cref="IFactorableToken<T>"/> out of <see cref="ParsedItem"/> array.
    /// </summary>
    /// <typeparam name="T">Token base type.</typeparam>
    class Tokenizer<T>
    {
        /// <summary>
        /// <see cref="ITokenFactory{T}"/> object used for actual <see cref="IFactorableToken{T}"/> creation.
        /// </summary>
        public ITokenFactory<T> TokenFactory { get; set; }

        private List<IFactorableToken<T>> tokens;
        private int currTokenIndex;

        private ParsedItem[] parsedItems;
        /// <summary>
        /// <see cref="ParsedItem"/>s to be tokenized.
        /// </summary>
        public ParsedItem[] DataToBeTokenized
        {
            set
            {
                Clear();
                if (value == null) { throw new ArgumentNullException("DataToBeTokenized"); }

                this.parsedItems = value;
                tokens.Capacity = parsedItems.Length / 2;
            }
        }

        /// <summary>
        /// Tokenized <see cref="DataToBeTokenized"/> (after <see cref="Tokenize"/> is called).
        /// </summary>
        public IFactorableToken<T>[] Tokens { get { return tokens.ToArray(); } }

        public Tokenizer()
        {
            tokens = new List<IFactorableToken<T>>();
            parsedItems = new ParsedItem[0];
            Clear();
        }

        /// <summary>
        /// Automatically sets <see cref="DataToBeTokenized"/> property.
        /// </summary>
        /// <param name="parsedItems">Data to be tokenized.</param>
        public Tokenizer(ParsedItem[] parsedItems) : this()
        {
            DataToBeTokenized = parsedItems;
        }

        /// <summary>
        /// Clears data in <see cref="Tokens"/> and resets <see cref="Tokenizer"/> state.
        /// </summary>
        public void Clear()
        {
            currTokenIndex = 0;
            tokens.Clear();
        }

        /// <summary>
        /// Tokenizes the <see cref="DataToBeTokenized"/> and appends the result to <see cref="Tokens"/>.
        /// </summary>
        /// <exception cref="TokenizerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Tokenize"/> is called without set <see cref="TokenFactory"/>.</exception>
        public void Tokenize()
        {
            if (TokenFactory == null) { throw new InvalidOperationException("Token factory not set."); }

            while (isCurrTokenIndexInRange())
            {
                tokens.Add(getToken());
                currTokenIndex++;
            }
        }

        private IFactorableToken<T> getToken()
        {
            switch (parsedItems[currTokenIndex].Type)
            {
                case ParsedItemType.Name:
                    return handleName();
                case ParsedItemType.Value:
                    return TokenFactory.CreateValue(parsedItems[currTokenIndex].Value);
                case ParsedItemType.LBracket:
                    return TokenFactory.CreateBrackets(extractTokensFromBrakets());
                case ParsedItemType.Operator:
                    return TokenFactory.CreateOperator(parsedItems[currTokenIndex].Value);
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
                return TokenFactory.CreateFunction(parsedItems[currTokenIndex].Value, extractTokensFromFunctionArgs());
            }
            else
            {
                return TokenFactory.CreateVariable(parsedItems[currTokenIndex].Value);
            }
        }

        private IFactorableToken<T>[][] extractTokensFromFunctionArgs()
        {
            currTokenIndex++;
            return extractTokensFromBrakets();
        }

        private IFactorableToken<T>[][] extractTokensFromBrakets()
        {
            if(isSomethingAfterCurrTokenIndex())
            {
                List<IFactorableToken<T>[]> arguments = new List<IFactorableToken<T>[]>();
                Tokenizer<T> argumentsTokenizer = new Tokenizer<T>() { TokenFactory = this.TokenFactory };

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
                        (bracketsLevel == 0 && parsedItems[currTokenIndex].Type == ParsedItemType.RBracket && length > 0))
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
