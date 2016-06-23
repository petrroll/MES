using MathExpressionSolver.Tokens;

namespace MathExpressionSolver.Interfaces
{
    public interface IExpTreeBuilder<T>
    {
        IToken<T> CreateExpressionTree(IFactorableToken<T>[] tokens);
    }
}
