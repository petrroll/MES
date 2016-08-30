using MathExpressionSolver.Parser;

namespace MathExpressionSolver.Interfaces
{
    public interface IParser
    {
        ParsedItem[] ParseExpression(string expression);
    }
}
