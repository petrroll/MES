using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionSolver.Tokens
{
    class ExpTreeBuilder<T>
    {

        IEnumerable<IFactorableToken<T>> tokenArray;

        public ExpTreeBuilder(IEnumerable<IFactorableToken<T>> tokenArray)
        {
            this.tokenArray = tokenArray;
        }

        public IToken<T> CreateExpressionTree()
        {
            Stack<IFactorableToken<T>> tokenStack = new Stack<IFactorableToken<T>>();

            IFactorableToken<T> lastToken = null;
            foreach (IFactorableToken<T> currToken in tokenArray)
            {
                while (tokenStack.Count > 0 && tokenStack.Peek().Priority >= currToken.Priority) { lastToken = tokenStack.Pop(); }

                if(lastToken != null)
                {
                    if(currToken.Type == TokenType.Brackets || currToken.Type == TokenType.Function)
                    {
                        ExpTreeBuilder<T> bracketedExpressionTree = new ExpTreeBuilder<T>(((IFactorableBracketsToken<T>)currToken).BracketedTokens);
                        ((IUnToken<T>)currToken).Child = bracketedExpressionTree.CreateExpressionTree();
                    }
                    else if (currToken.Type == TokenType.Operator)
                    {
                        if (tokenStack.Count > 0 && tokenStack.Peek().Type == TokenType.Operator) { ((BinOpToken<T>)tokenStack.Peek()).RightChild = currToken; }
                        ((BinOpToken<T>)currToken).LeftChild = lastToken;
                    }

                    if (currToken.Type != TokenType.Operator && lastToken.Type == TokenType.Operator)
                    {
                        ((BinOpToken<T>)lastToken).RightChild = currToken;
                    }
                }

                tokenStack.Push(currToken);
                lastToken = currToken;
            }
            return tokenStack.Last();
        }
    }
}
