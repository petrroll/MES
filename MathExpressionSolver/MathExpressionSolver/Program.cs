using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;
using System;

namespace MathExpressionSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            ProgramController controller = new ProgramController();
            controller.TestIfWorks();
        }
    }

    class ProgramController
    {
        ExpressionParser parser;
        Tokenizer tokenizer;
        ExpTreeBuilder<double> treeBuilder;

        public ProgramController()
        {
            parser = new ExpressionParser() { SkipWhiteSpace = true };
            tokenizer = new Tokenizer();
            treeBuilder = new ExpTreeBuilder<double>();
        }

        public void TestIfWorks()
        {
            testInput("3*(7+7)/2-2*6/7- &&&  (&6 +&9) *8-  (2+ 2/3*(6+exp  (2*7-6* 2 ) - 8)+(2>1))", "-107,306989780239");
            testInput("3*(7+7)/2-2*6/7-(6+9)*8-(2+2/3*(6+exp(2*7-6*2)-8)+(2>1))", "-107,306989780239");

            testInput("3 > 1", "1");
            testInput("((2+1)-(3+1)*2)", "-5");

            testInput("2&3 + 5", "8");
            Console.ReadLine();
        }

        private void testInput(string input, string output)
        {
            handleInput(input);
            prepareTree();
            testIfWorking(output);
        }

        public void Work()
        {
            while (true)
            {
                Console.WriteLine("Write math expression / declare function / save a variable and press enter.");
                Work(Console.ReadLine());
            }
        }

        public void Work(string s)
        {
            handleInput(s);
            prepareTree();
            writeOutResult();
        }

        private void handleInput(string input)
        {
            parser.StringExpression = input;
            parser.ParseExpression();
        }

        private void prepareTree()
        {
            tokenizer.SetDataToBeTokenized(parser.ParsedExpressions, parser.ParsedTypes);
            tokenizer.Tokenize();

            treeBuilder.RawTokens = tokenizer.Tokens;
            treeBuilder.CreateExpressionTree();  
        }

        private void testIfWorking(string exptectedResult)
        {
            string realResult = treeBuilder.TreeTop.ReturnValue().ToString();
            Console.WriteLine(exptectedResult == realResult);
        }

        private void writeOutResult()
        {
            string result = string.Empty;
            try
            {
                result = treeBuilder.TreeTop.ReturnValue().ToString();
            }
            catch (Exception ex)
            {
                result = "Expression not entered correctly.";
            }
            Console.WriteLine(result);
        }
    }
}
