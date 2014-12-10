using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionSolver.Parser
{
    class ExpressionParser
    {
        const int TKNLGHT = 4;
        public bool SkipInvalidChars { get; set; } = true;

        private StringBuilder charBuffer;
        private int bufferPointer = 0;

        private List<string> listOfParsedExpressions;
        private string stringExpression;

        public string StringExpression
        {
            set
            {
                listOfParsedExpressions.Clear();
                listOfParsedExpressions.Capacity = stringExpression.Length / TKNLGHT;
                stringExpression = value;
            }
        }

        public ExpressionParser() 
        {
            charBuffer = new StringBuilder(TKNLGHT);
            listOfParsedExpressions = new List<string>();
            stringExpression = string.Empty;
        }

        public ExpressionParser(string expression) : this()
        {
            StringExpression = expression;
        }

        public string[] ParseExpression()
        {
            parseExpression();
            return listOfParsedExpressions.ToArray();
        }

        private void parseExpression()
        {
            listOfParsedExpressions.Clear();
            while (bufferPointer < stringExpression.Length)
            {
                parseNextToken();
            }
        }

        private void parseNextToken()
        {
            charBuffer.Clear();
            Func<char, bool> isTypeFunction = null;

            bool isLong = false;
            bool trash = false;

            if (ParserHelper.IsNameChar(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
                isTypeFunction = ParserHelper.IsNameChar;
                isLong = true;
            }
            else if (ParserHelper.IsNum(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
                isTypeFunction = ParserHelper.IsNum;
                isLong = true;
            }
            else if (ParserHelper.IsWhiteSpace(stringExpression[bufferPointer]))
            {
                trashCurrChar();
                isTypeFunction = ParserHelper.IsWhiteSpace;
                isLong = true;
                trash = true;
            }
            else if (ParserHelper.IsLeftBracket(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
            }
            else if (ParserHelper.IsRightBracket(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
            }
            else if (ParserHelper.IsOperator(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
            }
            else if (ParserHelper.IsSeparator(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
            }
            else
            {
                trashCurrChar();
                if (SkipInvalidChars) return;
            }

            if(isLong)
            {
                while(bufferPointer < stringExpression.Length && isTypeFunction(stringExpression[bufferPointer]))
                {
                    if (trash) trashCurrChar();
                    else addCurrCharToBuffer();
                }
            }

            listOfParsedExpressions.Add(charBuffer.ToString());
        }

        private void addCurrCharToBuffer()
        {
            charBuffer.Append(stringExpression[bufferPointer]);
            bufferPointer++;
        }

        private void trashCurrChar()
        {
            bufferPointer++;
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
