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
        private IFactorableToken<T>[] rawTokens;
        /// <summary>
        /// Array of <see cref="IFactorableToken<T>"/> to be rebuild into an expression tree. 
        /// </summary>
        public IFactorableToken<T>[] RawTokens { set { Clear(); rawTokens = value; } }
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
        public ExpTreeBuilder(IFactorableToken<T>[] tokens) : this()
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
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        public void CreateExpressionTree()
        {
            Stack<IFactorableToken<T>> tokenStack = new Stack<IFactorableToken<T>>();

            foreach (IFactorableToken<T> currToken in rawTokens)
            {
                if (currToken.Type == TokenType.Brackets || currToken.Type == TokenType.Function)
                {
                    buildExpTreeInArguments((IFactorableBracketsToken<T>)currToken);
                }

                placeCurrentToken(tokenStack, currToken);

            }
            if (tokenStack.Count > 0) { TreeTop = tokenStack.Last(); } else { TreeTop = null; } //?Throw an exception?
        }

        private void placeCurrentToken(Stack<IFactorableToken<T>> tokenStack, IFactorableToken<T> currToken)
        {
            IFactorableToken<T> lastToken = (!tokenStack.IsEmpty()) ? tokenStack.Peek() : null;
            IToken<T> subHierarchyToken = null;
            while (!tokenStack.IsEmpty() && tokenStack.Peek().Priority >= currToken.Priority) { subHierarchyToken = tokenStack.Pop(); }
            IFactorableToken<T> upHierarchyToken = (!tokenStack.IsEmpty()) ? tokenStack.Peek() : null;

            if(lastToken?.Type == TokenType.BinOperator && currToken.Type == TokenType.BinOperator)
            {
                throw new ExpTreeBuilderException("Two binary operators next to each other: " + lastToken.ToString() + " " + currToken.ToString());
            }

            if (currToken.Type == TokenType.BinOperator && subHierarchyToken != null)
            {
                ((IBinToken<T>)currToken).LeftChild = subHierarchyToken;
            }
            else if(currToken.Type == TokenType.BinOperator)
            {
                throw new ExpTreeBuilderException("Binary operator: " + currToken.ToString() + " does't have left side");
            }

            if(upHierarchyToken?.Type == TokenType.BinOperator)
            {
                ((IBinToken<T>)upHierarchyToken).RightChild = currToken;
            }
            else if (!(tokenStack.IsEmpty() && 
                (currToken.Type == TokenType.BinOperator ^ subHierarchyToken == null)))
            {
                throw new ExpTreeBuilderException("Token " + currToken.ToString() + " isn't connected to any operator");
            }

            tokenStack.Push(currToken);
        }

        private void buildExpTreeInArguments(IFactorableBracketsToken<T> currToken)
        {
            ExpTreeBuilder<T> argumentTokenTreeBuilder = new ExpTreeBuilder<T>();
            IFactorableToken<T>[][] argumentsTokens = currToken.BracketedTokens;

            if (argumentsTokens.Length != currToken.Children.Length)
            {
                throw new ExpTreeBuilderException("Number of supplied (" + argumentsTokens.Length + ") and required (" + currToken.Children.Length + ") arguments for " + currToken.ToString() + " doesn't match.");
            }

            for (int nThArgument = 0; nThArgument < argumentsTokens.Length; nThArgument++)
            {
                argumentTokenTreeBuilder.RawTokens = argumentsTokens[nThArgument];
                argumentTokenTreeBuilder.CreateExpressionTree();
                currToken.Children[nThArgument] = argumentTokenTreeBuilder.TreeTop;
            }
        }
    }


    [System.Serializable]
    public class ExpTreeBuilderException : System.Exception
    {
        public ExpTreeBuilderException() { }
        public ExpTreeBuilderException(string message) : base(message) { }
        public ExpTreeBuilderException(string message, System.Exception inner) : base(message, inner) { }
        protected ExpTreeBuilderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
