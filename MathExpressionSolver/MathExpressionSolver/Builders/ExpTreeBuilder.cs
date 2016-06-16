using System;
using System.Collections.Generic;
using System.Linq;

namespace MathExpressionSolver.Tokens
{
    /// <summary>
    /// Converts linear array of <see cref="IFactorableToken{T}"/> into an expression tree.
    /// </summary>
    /// <typeparam name="T">Token base type.</typeparam>
    public class ExpTreeBuilder<T>
    {
        /// <summary>
        /// Creates an expression tree out of <paramref name="rawTokens"/> and returns its top Token.
        /// </summary>
        /// <param name="rawTokens"></param>
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        /// <returns>A top Token of created expression tree.</returns>
        public IToken<T> CreateExpressionTree(IFactorableToken<T>[] rawTokens)
        {
            var tokenStack = new Stack<IFactorableToken<T>>();

            foreach (IFactorableToken<T> currToken in rawTokens)
            {
                if (currToken.Type == TokenType.Brackets || currToken.Type == TokenType.Function)
                {
                    buildExpTreeInArguments((IFactorableBracketsToken<T>)currToken);
                }

                placeCurrentToken(tokenStack, currToken);

            }

            if (tokenStack.Count > 0)
            {
                if(tokenStack.Peek().Type == TokenType.BinOperator) { throw new ExpTreeBuilderException($"Binary operator: {tokenStack.Peek()} doesn't have right side."); }
                return tokenStack.Last();
            }
            else { return null; } 
        }

        private void placeCurrentToken(Stack<IFactorableToken<T>> tokenStack, IFactorableToken<T> currToken)
        {
            IFactorableToken<T> lastToken = (!tokenStack.IsEmpty()) ? tokenStack.Peek() : null;
            IToken<T> subHierarchyToken = null;
            while (!tokenStack.IsEmpty() && tokenStack.Peek().Priority >= currToken.Priority) { subHierarchyToken = tokenStack.Pop(); }
            IFactorableToken<T> upHierarchyToken = (!tokenStack.IsEmpty()) ? tokenStack.Peek() : null;

            if (lastToken?.Type == TokenType.BinOperator && currToken.Type == TokenType.BinOperator)
            {
                throw new ExpTreeBuilderException($"Two binary operators next to each other: {lastToken}, {currToken}");
            }

            if (currToken.Type == TokenType.BinOperator && subHierarchyToken != null)
            {
                ((IBinToken<T>)currToken).LeftChild = subHierarchyToken;
            }
            else if (currToken.Type == TokenType.BinOperator)
            {
                throw new ExpTreeBuilderException($"Binary operator: {currToken} does't have left side");
            }

            //Try to assign it under previous binary operator
            if (upHierarchyToken?.Type == TokenType.BinOperator)
            {
                ((IBinToken<T>)upHierarchyToken).RightChild = currToken;
            }
            //If stack isn't empty -> current token is disconected
            //If the current token type is binary & there's nothing to add as left side (or there's something to add as left side but current token is not binary) -> something is left left disconected.
            else if (
                (!tokenStack.IsEmpty()) || 
                ((currToken.Type == TokenType.BinOperator) == (subHierarchyToken == null))
                )
            {
                throw new ExpTreeBuilderException($"Token {currToken} isn't connected to any operator");
            }

            tokenStack.Push(currToken);
        }

        private void buildExpTreeInArguments(IFactorableBracketsToken<T> currToken)
        {
            var argumentTokenTreeBuilder = new ExpTreeBuilder<T>();
            IFactorableToken<T>[][] argumentsTokens = currToken.BracketedTokens;

            if (argumentsTokens.Length != currToken.Children.Length)
            {
                throw new ExpTreeBuilderException($"Number of supplied ({argumentsTokens.Length}) and required ({currToken.Children.Length}) arguments for {currToken} doesn't match.");
            }

            for (int nThArgument = 0; nThArgument < argumentsTokens.Length; nThArgument++)
            {
                var argumentTopToken = argumentTokenTreeBuilder.CreateExpressionTree(argumentsTokens[nThArgument]);

                if(argumentTopToken == null) { throw new ExpTreeBuilderException($"{nThArgument}. argument of {currToken} is empty." ); }
                currToken.Children[nThArgument] = argumentTopToken;
            }
        }
    }


    [System.Serializable]
    public class ExpTreeBuilderException : ExpressionException
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
