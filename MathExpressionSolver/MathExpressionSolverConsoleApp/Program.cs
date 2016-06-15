﻿using System;
using System.Collections.Generic;
using MathExpressionSolver.Builders;
using MathExpressionSolver.Controller;

using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;

namespace MathExpressionSolverConsoleApp
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
                Console.WriteLine(controller.ExecuteExpressionSafe(input));
            }
        }

        private static Controller<double> initController()
        {
            var parser = new ExpressionParser { SkipWhiteSpace = true };

            var customVariables = new Dictionary<string, double>();
            var customFunctions = new Dictionary<string, IFactorableBracketsToken<double>>();

            ITokenFactory<double> factory = new DoubleTokenFactory { CustomVariables = customVariables, CustomFunctions = customFunctions };
            var tokenizer = new Tokenizer<double> { TokenFactory = factory };

            var treeBuilder = new ExpTreeBuilder<double>();

            return new Controller<double> { ExpTreeBuilder = treeBuilder, Parser = parser, Tokenizer = tokenizer };
        }
    }
}
