using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;

namespace MathExpressionSolver.Interfaces
{
    /// <summary>
    /// Creates linear array of <see cref="IFactorableToken{T}"/> out of <see cref="ParsedItem"/> array.
    /// </summary>
    /// <typeparam name="T">Token base type.</typeparam>
    public interface ITokenizer<T>
    {
        /// <summary>
        /// <see cref="ITokenFactory{T}"/> object used for actual <see cref="IFactorableToken{T}"/> creation.
        /// </summary>
        ITokenFactory<T> TokenFactory { get; set; }

        /// <summary>
        /// Tokenizes the <paramref name="parsedItems"/> and returns an array of <see cref="IFactorableToken{T}"/>.
        /// </summary>
        /// <param name="parsedItems">Array of <see cref="ParsedItem"/>s to be tokenized.</param>
        /// <returns>Tokenized <paramref name="parsedItems"/></returns>
        IFactorableToken<T>[] Tokenize(ParsedItem[] parsedItems);
    }
}
