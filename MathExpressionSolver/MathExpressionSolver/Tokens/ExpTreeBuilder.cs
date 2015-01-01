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

            Func<IFactorableToken<T>> LastOnStack = () => { return tokenStack.Peek(); };
            Func<bool> IsStackEmpty = () => { return (tokenStack.Count == 0); };

            IFactorableToken<T> lastToken = null;
            foreach (IFactorableToken<T> currToken in RawTokens)
            {
                while (!IsStackEmpty() && LastOnStack().Priority >= currToken.Priority) { lastToken = tokenStack.Pop(); }

                if (currToken.Type == TokenType.Brackets || currToken.Type == TokenType.Function)
                {
                    ExpTreeBuilder<T> bracketedExpressionTree = new ExpTreeBuilder<T>(((IFactorableBracketsToken<T>)currToken).BracketedTokens);
                    bracketedExpressionTree.CreateExpressionTree();
                    ((IToken<T>)currToken).Children[0] = bracketedExpressionTree.TreeTop;
                }

                if (lastToken != null)
                {
                    if (currToken.Type == TokenType.BinOperator)
                    {
                        if (!IsStackEmpty() && LastOnStack().Type == TokenType.BinOperator) { ((IBinToken<T>)LastOnStack()).RightChild = currToken; }
                        ((IBinToken<T>)currToken).LeftChild = lastToken;
                    }
                    else if (currToken.Type != TokenType.BinOperator && lastToken.Type == TokenType.BinOperator)
                    {
                        ((IBinToken<T>)lastToken).RightChild = currToken;
                    }
                }

                tokenStack.Push(currToken);
                lastToken = currToken;
            }
            TreeTop = tokenStack.Last();
        }
    }
}
