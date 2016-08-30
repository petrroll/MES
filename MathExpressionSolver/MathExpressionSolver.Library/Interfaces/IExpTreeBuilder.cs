using MathExpressionSolver.Tokens;

namespace MathExpressionSolver.Interfaces
{

    /// <summary>
    /// Converts linear array of <see cref="IFactorableToken{T}"/> into an expression tree.
    /// </summary>
    /// <typeparam name="T">Token base type.</typeparam>
    public interface IExpTreeBuilder<T>
    {
        /// <summary>
        /// Creates an expression tree out of <paramref name="tokens"/> and returns its top Token.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns>A top Token of created expression tree.</returns>
        IToken<T> CreateExpressionTree(IFactorableToken<T>[] tokens);
    }
}
