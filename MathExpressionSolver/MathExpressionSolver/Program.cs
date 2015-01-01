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
            controller.Work("3*(7+7)/2-2*6/7-(6+9)*8-(2+2/3*(6+exp(2*7-6*2)-8)+2)");
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
            Console.WriteLine("Write math expression / declare function / save a variable and press enter.");
            Work(Console.ReadLine());
        }

        public void Work(string s)
        {
            handleInput(s);
            Console.WriteLine(computeResult());
        }

        private void handleInput(string input)
        {
            parser.StringExpression = input;
            parser.ParseExpression();
        }

        private double computeResult()
        {
            tokenizer.SetDataToBeTokenized(parser.ParsedExpressions, parser.ParsedTypes);
            tokenizer.Tokenize();

            treeBuilder.RawTokens = tokenizer.Tokens;
            treeBuilder.CreateExpressionTree();

            return treeBuilder.TreeTop.ReturnValue();   
        }
    }


}
