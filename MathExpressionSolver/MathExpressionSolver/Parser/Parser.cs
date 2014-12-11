using System;
using System.Collections.Generic;
using System.Text;

namespace MathExpressionSolver.Parser
{
    class ExpressionParser
    {
        const int TKNLGHT = 4;
        public bool SkipInvalidChars { get; set; } = true;

        private StringBuilder charBuffer;
        private int bufferPointer = 0;

        private List<string> listOfParsedExpressions;
        private List<ParsedSubstringType> listOfParsedTypes;
        private string stringExpression;

        public string StringExpression
        {
            set
            {
                listOfParsedExpressions.Clear();
                listOfParsedExpressions.Capacity = stringExpression.Length / TKNLGHT;

                listOfParsedTypes.Clear();
                listOfParsedTypes.Capacity = stringExpression.Length / TKNLGHT;

                stringExpression = value;
            }
        }

        public ExpressionParser() 
        {
            charBuffer = new StringBuilder(TKNLGHT);

            listOfParsedExpressions = new List<string>();
            listOfParsedTypes = new List<ParsedSubstringType>();

            stringExpression = string.Empty;
        }

        public ExpressionParser(string expression) : this()
        {
            StringExpression = expression;
        }

        public Tuple<string[], ParsedSubstringType[]> ParseExpression()
        {
            parseExpression();
            return new Tuple<string[], ParsedSubstringType[]>(listOfParsedExpressions.ToArray(), listOfParsedTypes.ToArray());
        }

        private void parseExpression()
        {
            listOfParsedExpressions.Clear();
            listOfParsedTypes.Clear();

            while (isNotAtEnd())
            {
                parseNextToken();
            }
        }

        private void parseNextToken()
        {
            charBuffer.Clear();
            Func<char, bool> isTypeFunction = null;
            ParsedSubstringType currentType;

            bool isLong = false;
            bool trash = false;

            if (ParserHelper.IsNameChar(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
                isTypeFunction = ParserHelper.IsNameChar;
                currentType = ParsedSubstringType.Name;
                isLong = true;
            }
            else if (ParserHelper.IsNum(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
                isTypeFunction = ParserHelper.IsNum;
                currentType = ParsedSubstringType.Num;
                isLong = true;
            }
            else if (ParserHelper.IsWhiteSpace(stringExpression[bufferPointer]))
            {
                trashCurrChar();
                isTypeFunction = ParserHelper.IsWhiteSpace;
                currentType = ParsedSubstringType.WhiteSpace;
                isLong = true;
                trash = true;
            }
            else if (ParserHelper.IsLeftBracket(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
                currentType = ParsedSubstringType.Bracket;
            }
            else if (ParserHelper.IsRightBracket(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
                currentType = ParsedSubstringType.Bracket;
            }
            else if (ParserHelper.IsOperator(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
                currentType = ParsedSubstringType.Operator;
            }
            else if (ParserHelper.IsSeparator(stringExpression[bufferPointer]))
            {
                addCurrCharToBuffer();
                currentType = ParsedSubstringType.Separator;
            }
            else
            {
                trashCurrChar();
                if (SkipInvalidChars) return;
                else currentType = ParsedSubstringType.WhiteSpace;
            }

            if(isLong)
            {
                while(isNotAtEnd() && isTypeFunction(stringExpression[bufferPointer]))
                {
                    if (trash) trashCurrChar();
                    else addCurrCharToBuffer();
                }
            }

            listOfParsedExpressions.Add(charBuffer.ToString());
            listOfParsedTypes.Add(currentType);
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
        private bool isNotAtEnd()
        {
            return bufferPointer < stringExpression.Length;
        }
    }

    public enum ParsedSubstringType { Name, Num, Bracket, Operator, Separator, WhiteSpace };

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
