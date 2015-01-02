using System;
using System.Collections.Generic;
using System.Linq;

namespace MathExpressionSolver.Tokens
{
    class ExpTreeBuilder<T>
    {
        private IEnumerable<IFactorableToken<T>> rawTokens;
        public IEnumerable<IFactorableToken<T>> RawTokens { private get { return rawTokens; } set { rawTokens = value; TreeTop = null; } }

        public IToken<T> TreeTop { get; set; }

        public ExpTreeBuilder()
        {
            rawTokens = new IFactorableToken<T>[0];
        }

        public ExpTreeBuilder(IEnumerable<IFactorableToken<T>> tokens) : this()
        {
            RawTokens = tokens;
        }

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
            TreeTop = tokenStack.Last();
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
            ExpTreeBuilder<T> arguementTokenTreeBuilder = new ExpTreeBuilder<T>();
            IEnumerable<IFactorableToken<T>>[] arguementsTokens = ((IFactorableBracketsToken<T>)currToken).BracketedTokens;

            int nThArguement = 0;
            while (arguementsTokens.Length > nThArguement &&
                currToken.Children.Length > nThArguement)
            {
                arguementTokenTreeBuilder.RawTokens = arguementsTokens[nThArguement];
                arguementTokenTreeBuilder.CreateExpressionTree();
                currToken.Children[nThArguement] = arguementTokenTreeBuilder.TreeTop;

                nThArguement++;
            }
        }
    }
}
