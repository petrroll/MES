using System;
using System.Collections.Generic;
using MathExpressionSolver.Builders;
using MathExpressionSolver.Controller;

using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;
using MathExpressionSolver.Interfaces;

namespace MESExamples.CosnsoleApp
{
    class Program
    {
#pragma warning disable RECS0154 // Parameter is never used
        static void Main(string[] args)
#pragma warning restore RECS0154 // Parameter is never used
        {
            string input;
            Controller<double> controller = initController();

            while (true)
            {
                input = Console.ReadLine();
                if(input == "--bye") { break; }
                Console.WriteLine(controller.ExecuteExpressionSafe(input));
            }
        }

        private static Controller<double> initController()
        {
            var parser = new ExpressionParser { SkipWhiteSpace = true };

            var customVariables = new Dictionary<string, double>();
            var customFunctions = new Dictionary<string, IFactorableCustFuncToken<double>>();

            ITokenFactory<double> factory = new DoubleTokenFactory { CustomVariables = customVariables, CustomFunctions = customFunctions };
            var tokenizer = new Tokenizer<double> { TokenFactory = factory };

            var treeBuilder = new ExpTreeBuilder<double>();

            return new Controller<double> { ExpTreeBuilder = treeBuilder, Parser = parser, Tokenizer = tokenizer };
        }
    }
}
