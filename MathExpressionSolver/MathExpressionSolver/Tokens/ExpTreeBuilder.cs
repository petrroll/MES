using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionSolver.Tokens
{
    class ExpTreeBuilder
    {

        IToken<int>[] tokenArray;

        public ExpTreeBuilder(IToken<int>[] tokenArray)
        {
            this.tokenArray = tokenArray;
        }

        public IToken<int> CreateExpressionTree()
        {
            Stack<IToken<int>> tokenStack = new Stack<IToken<int>>();

            IToken<int> lastToken = null;
            foreach (IToken<int> currToken in tokenArray)
            {
                while (tokenStack.Count > 0 && tokenStack.Peek().Priority >= currToken.Priority) { lastToken = tokenStack.Pop(); }

                if(lastToken != null)
                {
                    if (currToken.Type == TokenType.Operator)
                    {
                        if(tokenStack.Count > 0 && tokenStack.Peek().Type == TokenType.Operator) { ((IntBinOpToken)tokenStack.Peek()).RightChild = currToken; }
                        ((IntBinOpToken)currToken).LeftChild = lastToken;
                    }
                    else if (lastToken.Type == TokenType.Operator)
                    {
                        ((IntBinOpToken)lastToken).RightChild = currToken;
                    }
                }

                tokenStack.Push(currToken);
                lastToken = currToken;
            }
            return tokenStack.Last();
        }
    }
}
