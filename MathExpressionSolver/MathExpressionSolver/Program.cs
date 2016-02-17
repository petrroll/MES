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
                if (input == "!TEST!") testSystem(controller);
                else Console.WriteLine(controller.ExecuteExpressionSafe(input));
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

        private static void testSystem(Controller<double> contr)
        {
            testInput("3*(7+7)/2-2*6/7- &&&  (&6 +&9) *8-  (2+ 2/3*(6+exp  (2*7-6* 2 ) - 8)+(2>1))", "-107,306989780239", contr);
            testInput("3*(7+7)/2-2*6/7-(6+9)*8-(2+2/3*(6+exp(2*7-6*2)-8)+(2>1))", "-107,306989780239", contr);

            testInput("3 > 1", "1", contr);
            testInput("((2+1)-(3+1)*2)", "-5", contr);

            testInput("a=(3/6*5)", "a = 2,5", contr);
            testInput("asdfsdf=(5 + 3)", "asdfsdf = 8", contr);

            testInput("Pi = 3,4", "Pi = 3,4", contr);
            testInput("3 - Pi + 8", "7,6", contr);

            testInput("a", "2,5", contr);
            testInput("asdfsdf", "8", contr);

            testInput("exp(a) + asdfsdf - 2*Pi", "13,3824939607035", contr);

            testInput("if(1;2;3)", "2", contr);
            testInput("if(0;2;3)", "3", contr);

            testInput("if((exp(100)> Pi)*2;exp(a) + asdfsdf - 2*Pi;2*3-asdfsdf)", "13,3824939607035", contr);
            testInput("(if((exp(100)> Pi)*2;exp(a) + asdfsdf - 2*Pi;2*3-asdfsdf))*2>1", "1", contr);

            testInput("funA(a;b) = a + b", "Function funA set.", contr);
            testInput("funA(2;5)", "7", contr);

            testInput("funB(height_in_m; diameter_in_cm) = Pi * ((diameter_in_cm / 100) * (diameter_in_cm / 100)) * height_in_m", "Function funB set.", contr);
            testInput("funB(20; 8)", "0,4352", contr);
            testInput("funB(Pi; sin(20))", "0,000963490199635007", contr);
        }

        private static void testInput(string expression, string wantedResult, Controller<double> controller)
        {
            string result = controller.ExecuteExpression(expression);
            Console.WriteLine("{0} | {1} -EQ {2}", result == wantedResult, result, wantedResult);
        }
    }
}
