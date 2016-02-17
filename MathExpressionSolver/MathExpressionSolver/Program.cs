using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;
using System;
using System.Collections.Generic;
using MathExpressionSolver.Controller;

namespace MathExpressionSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            string input;
            Controller<double> controller = initController();

            while (true)
            {
                input = Console.ReadLine();
                controller.ExecuteExpressionSafe(input);
            }
        }

        private static Controller<double> initController()
        {
            var parser = new ExpressionParser { SkipWhiteSpace = true, SkipInvalidChars = true };

            var customVariables = new Dictionary<string, double>();
            var customFunctions = new Dictionary<string, IFactorableBracketsToken<double>>();

            TokenFactory<double> factory = new DoubleTokenFactory { CustomVariables = customVariables, CustomFunctions = customFunctions };
            var tokenizer = new Tokenizer<double> { TokenFactory = factory };

            var treeBuilder = new ExpTreeBuilder<double>();

            return new Controller<double> { ExpTreeBuilder = treeBuilder, Parser = parser, Tokenizer = tokenizer };
        }
    }
}
