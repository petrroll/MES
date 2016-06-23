using MathExpressionSolver.Builders;
using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;

namespace MathExpressionSolver.Interfaces
{
    public interface ITokenizer<T>
    {
        ITokenFactory<T> TokenFactory { get; set; }
        IFactorableToken<T>[] Tokenize(ParsedItem[] parsedItems);
    }
}
