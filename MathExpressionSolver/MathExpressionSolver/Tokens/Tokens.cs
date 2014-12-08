using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionSolver.Tokens
{

    public interface IToken<T>
    {
        T ReturnValue();
        int Priority { get; }
    }

    public abstract class Token<T> : IToken<T>
    {
        virtual public int Priority { get; protected set; }
        abstract public T ReturnValue();
    }

   
}
