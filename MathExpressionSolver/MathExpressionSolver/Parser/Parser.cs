using System;
using System.Collections.Generic;
using System.Text;

namespace MathExpressionSolver.Parser
{
    class ExpressionParser
    {
        const int avgTokenLength = 4;
        public bool SkipInvalidChars { get; set; } = true;

        private StringBuilder expressionBuffer;
        private int currCharPointer = 0;

        private List<string> parsedExpressions;
        private List<ParsedSubstringType> parsedTypes;
        private string rawExpression;

        public string StringExpression
        {
            set
            {
                parsedExpressions.Clear();
                parsedExpressions.Capacity = rawExpression.Length / avgTokenLength;

                parsedTypes.Clear();
                parsedTypes.Capacity = rawExpression.Length / avgTokenLength;

                rawExpression = value;
            }
        }

        public ExpressionParser() 
        {
            expressionBuffer = new StringBuilder(avgTokenLength);

            parsedExpressions = new List<string>();
            parsedTypes = new List<ParsedSubstringType>();

            rawExpression = string.Empty;
        }

        public ExpressionParser(string expression) : this()
        {
            StringExpression = expression;
        }

        public Tuple<string[], ParsedSubstringType[]> ParseExpression()
        {
            parseExpression();
            return new Tuple<string[], ParsedSubstringType[]>(parsedExpressions.ToArray(), parsedTypes.ToArray());
        }

        private void parseExpression()
        {
            parsedExpressions.Clear();
            parsedTypes.Clear();

            while (isNotAtEnd())
            {
                parseNextToken();
            }
        }

        private void parseNextToken()
        {
            expressionBuffer.Clear();
            Func<char, bool> isTypeFunction = null;
            ParsedSubstringType currentType;

            bool isLong = false;
            bool trash = false;

            if (ParserHelper.IsNameChar(rawExpression[currCharPointer]))
            {
                addCurrCharToBuffer();
                isTypeFunction = ParserHelper.IsNameChar;
                currentType = ParsedSubstringType.Name;
                isLong = true;
            }
            else if (ParserHelper.IsNum(rawExpression[currCharPointer]))
            {
                addCurrCharToBuffer();
                isTypeFunction = ParserHelper.IsNum;
                currentType = ParsedSubstringType.Num;
                isLong = true;
            }
            else if (ParserHelper.IsWhiteSpace(rawExpression[currCharPointer]))
            {
                trashCurrChar();
                isTypeFunction = ParserHelper.IsWhiteSpace;
                currentType = ParsedSubstringType.WhiteSpace;
                isLong = true;
                trash = true;
            }
            else if (ParserHelper.IsLeftBracket(rawExpression[currCharPointer]))
            {
                addCurrCharToBuffer();
                currentType = ParsedSubstringType.LBracket;
            }
            else if (ParserHelper.IsRightBracket(rawExpression[currCharPointer]))
            {
                addCurrCharToBuffer();
                currentType = ParsedSubstringType.RBracket;
            }
            else if (ParserHelper.IsOperator(rawExpression[currCharPointer]))
            {
                addCurrCharToBuffer();
                currentType = ParsedSubstringType.Operator;
            }
            else if (ParserHelper.IsSeparator(rawExpression[currCharPointer]))
            {
                addCurrCharToBuffer();
                currentType = ParsedSubstringType.Separator;
            }
            else
            {
                trashCurrChar();
                if (SkipInvalidChars) { return; }
                else { currentType = ParsedSubstringType.InvalidToken; }
            }

            if(isLong)
            {
                while (isNotAtEnd() && isTypeFunction(rawExpression[currCharPointer]))
                {
                    if (trash) { trashCurrChar(); }
                    else { addCurrCharToBuffer(); }
                }
            }

            parsedExpressions.Add(expressionBuffer.ToString());
            parsedTypes.Add(currentType);
        }

        private void addCurrCharToBuffer()
        {
            expressionBuffer.Append(rawExpression[currCharPointer]);
            movePointer();
        }

        private void trashCurrChar()
        {
            movePointer();
        }

        private void movePointer()
        {
            currCharPointer++;
        }

        private bool isNotAtEnd()
        {
            return (currCharPointer < rawExpression.Length);
        }
    }

    public enum ParsedSubstringType { Name, Num, LBracket, RBracket, Operator, Separator, WhiteSpace, InvalidToken };

    public static class ParserHelper
    {
        public static bool IsNameChar(char a)
        {
            if (a == '_') { return true; }
            else { return char.IsLetter(a); }
        }

        public static bool IsNum(char a)
        {
            if (a == '.') { return true; }
            else { return char.IsDigit(a); }
        }

        public static bool IsLeftBracket(char a)
        {
            return (a == '(');
        }

        public static bool IsRightBracket(char a)
        {
            return (a == ')');
        }

        public static bool IsOperator(char a)
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
            return (a == ';'); 
        }

        public static bool IsWhiteSpace(char a)
        {
            return char.IsWhiteSpace(a);
        }
    }
}
