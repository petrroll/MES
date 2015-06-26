using System.Collections.Generic;
using System.Linq;

namespace MathExpressionSolver.Tokens
{
    /// <summary>
    /// Converts linear array of <see cref="IFactorableToken"/> into an expression tree.
    /// </summary>
    /// <typeparam name="T">Token base type.</typeparam>
    class ExpTreeBuilder<T>
    {
        private IEnumerable<IFactorableToken<T>> rawTokens;
        /// <summary>
        /// Array of <see cref="IFactorableToken<T>"/> to be rebuild into an expression tree. 
        /// </summary>
        public IEnumerable<IFactorableToken<T>> RawTokens { private get { return rawTokens; } set { rawTokens = value; Clear(); } }
        /// <summary>
        /// The top <see cref="IToken{T}"/> of expression tree determined by <see cref="CreateExpressionTree"/>.
        /// </summary>
        public IToken<T> TreeTop { get; private set; }

        public ExpTreeBuilder()
        {
            rawTokens = new IFactorableToken<T>[0];
            Clear();
        }

        /// <summary>
        /// Automatically sets <see cref="RawTokens"/> property.
        /// </summary>
        /// <param name="tokens">Array of <see cref="IFactorableToken<T>"/> to be rebuild into an expression tree.</param>
        public ExpTreeBuilder(IEnumerable<IFactorableToken<T>> tokens) : this()
        {
            RawTokens = tokens;
        }

        /// <summary>
        /// Cleares <see cref="TreeTop"/> and resets <see cref="ExpTreeBuilder{T}"/> state.
        /// </summary>
        public void Clear()
        {
            TreeTop = null;
        }

        /// <summary>
        /// Creates an expression tree out of <see cref="RawTokens"/> and puts its top to <see cref="TreeTop"/>.
        /// </summary>
        public void CreateExpressionTree()
        {
            Stack<IFactorableToken<T>> tokenStack = new Stack<IFactorableToken<T>>();

            IFactorableToken<T> lastToken = null;
            foreach (IFactorableToken<T> currToken in RawTokens)
            {
                if (currToken.Type == TokenType.Brackets || currToken.Type == TokenType.Function)
                {
                    buildExpTreeInArguments(currToken);
                }

                if (lastToken != null)
                {
                    placeCurrentToken(tokenStack, lastToken, currToken);
                }

                tokenStack.Push(currToken);
                lastToken = currToken;
            }
            if (tokenStack.Count > 0) { TreeTop = tokenStack.Last(); } else { TreeTop = null; }
        }

        private static void placeCurrentToken(Stack<IFactorableToken<T>> tokenStack, IFactorableToken<T> lastToken, IFactorableToken<T> currToken)
        {
            while (!tokenStack.IsEmpty() && tokenStack.Peek().Priority >= currToken.Priority) { lastToken = tokenStack.Pop(); }

            if (currToken.Type == TokenType.BinOperator)
            {
                if (!tokenStack.IsEmpty() && tokenStack.Peek().Type == TokenType.BinOperator) { ((IBinToken<T>)tokenStack.Peek()).RightChild = currToken; }
                ((IBinToken<T>)currToken).LeftChild = lastToken;
            }
            else if (currToken.Type != TokenType.BinOperator && lastToken.Type == TokenType.BinOperator)
            {
                ((IBinToken<T>)lastToken).RightChild = currToken;
            }
        }

        private static void buildExpTreeInArguments(IFactorableToken<T> currToken)
        {
            ExpTreeBuilder<T> argumentTokenTreeBuilder = new ExpTreeBuilder<T>();
            IEnumerable<IFactorableToken<T>>[] argumentsTokens = ((IFactorableBracketsToken<T>)currToken).BracketedTokens;

            int nThArgument = 0;
            while (argumentsTokens.Length > nThArgument &&
                currToken.Children.Length > nThArgument)
            {
                argumentTokenTreeBuilder.RawTokens = argumentsTokens[nThArgument];
                argumentTokenTreeBuilder.CreateExpressionTree();
                currToken.Children[nThArgument] = argumentTokenTreeBuilder.TreeTop;

                nThArgument++;
            }
        }
    }
}
