using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionSolver.Parser
{
    class Parser
    {
        const int TKNLGHT = 4;

        private string expression;
        private int bufferPointer = 0;

        public Parser(string expression)
        {
            this.expression = expression;
        }

        public string[] ParseExpression()
        {
            List<string> listOfParsedExpressions = new List<string>(expression.Length / TKNLGHT);
            while(bufferPointer < expression.Length)
            {
                listOfParsedExpressions.Add(parseNextToken());
            }
            return listOfParsedExpressions.ToArray();
        }

        private string parseNextToken()
        {
            StringBuilder item = new StringBuilder(TKNLGHT);
            Func<char, bool> isTypeFunction = null;

            bool isLong = false;
            bool trash = false;

            if (ParserHelper.IsNameChar(expression[bufferPointer]))
            {
                isTypeFunction = ParserHelper.IsNameChar;
                isLong = true;
            }
            else if (ParserHelper.IsNum(expression[bufferPointer]))
            {
                isTypeFunction = ParserHelper.IsNum;
                isLong = true;
            }
            else if (ParserHelper.IsWhiteSpace(expression[bufferPointer]))
            {
                isTypeFunction = ParserHelper.IsWhiteSpace;
                isLong = true;
                trash = true;
            }
            else if (ParserHelper.IsLeftBracket(expression[bufferPointer]))
            {
                isTypeFunction = ParserHelper.IsLeftBracket;
            }
            else if (ParserHelper.IsRightBracket(expression[bufferPointer]))
            {
                isTypeFunction = ParserHelper.IsRightBracket;
            }
            else if (ParserHelper.IsOperator(expression[bufferPointer]))
            {
                isTypeFunction = ParserHelper.IsOperator;
            }
            else if (ParserHelper.IsSeparator(expression[bufferPointer]))
            {
                isTypeFunction = ParserHelper.IsSeparator;
            }
            else
            {
                bufferPointer++;
                return string.Empty;
            }

            if(isLong)
            {
                while(bufferPointer < expression.Length && isTypeFunction(expression[bufferPointer]))
                {
                    if(!trash) item.Append(expression[bufferPointer]);
                    bufferPointer++;
                }
            }
            else
            {
                if (!trash) item.Append(expression[bufferPointer]);
                bufferPointer++;
            }
 
            return item.ToString();
        }
    }

    public static class ParserHelper
    {
        public static bool IsNameChar(char a)
        {
            if(a == '_') return true;
            else return char.IsLetter(a);
        }

        public static bool IsNum(char a)
        {
            if (a == '.') return true;
            else return char.IsDigit(a);
        }

        public static bool IsLeftBracket(char a)
        {
            return (a == '(') ? true : false;
        }

        public static bool IsRightBracket(char a)
        {
            return (a == ')') ? true : false;
        }

        public static bool IsOperator (char a)
        {
            switch (a)
            {
                case '+':
                    return true;
                case '-':
                    return true;
                case '*':
                    return true;
                case '/':
                    return true;
                case '%':
                    return true;
                case '=':
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSeparator(char a)
        {
            return (a == ';') ? true : false; 
        }

        public static bool IsWhiteSpace(char a)
        {
            return (char.IsWhiteSpace(a)) ? true : false;
        }
    }
}
