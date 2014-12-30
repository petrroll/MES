using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionSolver.Tokens
{
    class ExpTreeBuilder<T>
    {

        IToken<T>[] tokenArray;

        public ExpTreeBuilder(IToken<T>[] tokenArray)
        {
            this.tokenArray = tokenArray;
        }

        public IToken<T> CreateExpressionTree()
        {
            Stack<IToken<T>> tokenStack = new Stack<IToken<T>>();

            IToken<T> lastToken = null;
            foreach (IToken<T> currToken in tokenArray)
            {
                while (tokenStack.Count > 0 && tokenStack.Peek().Priority >= currToken.Priority) { lastToken = tokenStack.Pop(); }

                if(lastToken != null)
                {
                    if (currToken.Type == TokenType.Operator)
                    {
                        if(tokenStack.Count > 0 && tokenStack.Peek().Type == TokenType.Operator) { ((BinOpToken<T>)tokenStack.Peek()).RightChild = currToken; }
                        ((BinOpToken<T>)currToken).LeftChild = lastToken;
                    }
                    else if (lastToken.Type == TokenType.Operator)
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
