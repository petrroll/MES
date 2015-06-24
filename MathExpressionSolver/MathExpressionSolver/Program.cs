using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;
using System;

namespace MathExpressionSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            string input;
            ProgramController<double> controller = new ProgramController<double>();
            while (true)
            {
                input = Console.ReadLine();
                if (input == "!TEST!") controller.TestIfWorks();
                else controller.Work(input);
            }
        }
    }

    class ProgramController<T> 
    {
        ExpressionParser parser;
        ParsedItem[] parsedItems;

        Tokenizer<T> tokenizer;
        ExpTreeBuilder<T> treeBuilder;

        StorageHandler<T> storageHandler;

        public ProgramController()
        {
            parser = new ExpressionParser() { SkipWhiteSpace = true };
            tokenizer = new Tokenizer<T>();
            treeBuilder = new ExpTreeBuilder<T>();
    
            storageHandler = new StorageHandler<T>();

            tokenizer.CustomVariables = storageHandler.Variables;
        }

        public void Work(string input)
        {
            handleInput(input);
            work();
            writeOutResult();
        }

        public void TestIfWorks()
        {
            testInput("3*(7+7)/2-2*6/7- &&&  (&6 +&9) *8-  (2+ 2/3*(6+exp  (2*7-6* 2 ) - 8)+(2>1))", "-107,306989780239");
            testInput("3*(7+7)/2-2*6/7-(6+9)*8-(2+2/3*(6+exp(2*7-6*2)-8)+(2>1))", "-107,306989780239");

            testInput("3 > 1", "1");
            testInput("((2+1)-(3+1)*2)", "-5");

            testInput("2&3 + 5", "8");

            testInput("a=(3/6*5)", "2,5");
            testInput("asdfsdf=(5 + 3)", "8");

            testInput("Pi = 3,4", "3,4");
            testInput("3 - Pi + 8", "7,6");

            testInput("a", "2,5");
            testInput("asdfsdf", "8");

            testInput("exp(a) + asdfsdf - 2*Pi", "13,3824939607035");

            testInput("if(1;2;3)", "2");
            testInput("if(0;2;3)", "3");

            testInput("if((exp(100)> Pi)*2;exp(a) + asdfsdf - 2*Pi;2*3-asdfsdf)", "13,3824939607035");
            testInput("(if((exp(100)> Pi)*2;exp(a) + asdfsdf - 2*Pi;2*3-asdfsdf))*2>1", "1");
        }

        private void testInput(string input, string output)
        {
            handleInput(input);
            work();
            testIfWorking(output);
        }

        private void handleInput(string input)
        {
            parser.StringExpression = input;
            parser.ParseExpression();

            parsedItems = parser.ParsedItems;

        }

        private void work()
        {
            bool isVariable = false;
            bool isFunctrion = false;

            string variableName = string.Empty;
            string functionName = string.Empty;

            if(parsedItems[0].Type == ParsedItemType.Name)
            {
                if (parsedItems.Length > 1 &&
                    (parsedItems[1].Type == ParsedItemType.Operator && parsedItems[1].Value == "="))
                {
                    isVariable = true;
                    variableName = parsedItems[0].Value;

                    parsedItems = parsedItems.SubArray(2);
                }
            }

            prepareTree();

            if(isVariable)
            {
                try
                {
                    T result = computeResult();
                    storageHandler.AddVariable(variableName, result);
                }
                catch
                {
                    
                }
  
            }
        }

        private void prepareTree()
        {
            tokenizer.DataToBeTokenized = parsedItems;
            tokenizer.Tokenize();

            treeBuilder.RawTokens = (IFactorableToken<T>[])tokenizer.Tokens;
            treeBuilder.CreateExpressionTree();  
        }

        private void testIfWorking(string exptectedResult)
        {
            string realResult = computeResult().ToString();
            bool isOk = (exptectedResult == realResult);
            Console.WriteLine(isOk.ToString() + " | " + realResult);

            if (!isOk)
            {
                //Breakpoint;
            }
        }

        private T computeResult()
        {
            return treeBuilder.TreeTop.ReturnValue();
        }

        private void writeOutResult()
        {
            string result = string.Empty;
            try
            {
                result = computeResult().ToString();
            }
            catch (Exception ex)
            {
                result = "Expression not entered correctly.";
            }
            Console.WriteLine(result);
        }
    }
}
