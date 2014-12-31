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
                Func<IFactorableToken<T>> LastOnStack = () => { return tokenStack.Peek(); };
                Func<bool> IsStackEmpty = () => { return (tokenStack.Count == 0); };

                while (!IsStackEmpty() && LastOnStack().Priority >= currToken.Priority) { lastToken = tokenStack.Pop(); }

                if (lastToken != null)
                {
                    if (currToken.Type == TokenType.Brackets || currToken.Type == TokenType.Function)
                    {
                        ExpTreeBuilder<T> bracketedExpressionTree = new ExpTreeBuilder<T>(((IFactorableBracketsToken<T>)currToken).BracketedTokens);
                        bracketedExpressionTree.CreateExpressionTree();
                        ((IUnToken<T>)currToken).Child = bracketedExpressionTree.TreeTop;
                    }
                    else if (currToken.Type == TokenType.BinOperator)
                    {
                        if (!IsStackEmpty() && LastOnStack().Type == TokenType.BinOperator) { ((BinOpToken<T>)LastOnStack()).RightChild = currToken; }
                        ((BinOpToken<T>)currToken).LeftChild = lastToken;
                    }

                    if (currToken.Type != TokenType.BinOperator && lastToken.Type == TokenType.BinOperator)
                    {
                        ((BinOpToken<T>)lastToken).RightChild = currToken;
                    }
                }

                tokenStack.Push(currToken);
                lastToken = currToken;
            }
            TreeTop = tokenStack.Last();
        }
    }
}
