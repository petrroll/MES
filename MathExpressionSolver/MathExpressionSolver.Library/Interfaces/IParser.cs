using MathExpressionSolver.Parser;

namespace MathExpressionSolver.Interfaces
{

    /// <summary>
    /// Divides expression string into an array of substrings and their types.
    /// </summary>
    public interface IParser
    {

        /// <summary>
        /// Parses the <paramref name="rawExpression"/> and returns an array of corresponding <see cref="ParsedItem"/>s.
        /// </summary>
        /// <param name="rawExpression">Expression to be parsed.</param>
        /// <returns>Array of <see cref="ParsedItem"/>s</returns>
        ParsedItem[] ParseExpression(string expression);
    }
}
