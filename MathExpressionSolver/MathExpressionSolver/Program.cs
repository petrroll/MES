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
            controller.Work("(3>2)+5>1+2*3");
        }
    }

    class ProgramController
    {
        ExpressionParser parser;
        Tokenizer tokenizer;
        ExpTreeBuilder<double> treeBuilder;

        public ProgramController()
        {
            parser = new ExpressionParser();
            tokenizer = new Tokenizer();
            treeBuilder = new ExpTreeBuilder<double>();
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
