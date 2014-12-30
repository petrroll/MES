using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MathExpressionSolver.Parser
{
    class ExpressionParser
    {
        const int avgParsedItemLength = 4;
        public bool SkipInvalidChars { get; set; } = false;


        private List<string> parsedExpressions;
        private List<ParsedItemType> parsedTypes;

        public string[] ParsedExpression { get { return parsedExpressions.ToArray(); } }
        public ParsedItemType[] ParsedTypes { get { return parsedTypes.ToArray(); } }

        private string rawExpression;

        public string StringExpression
        {
            set
            {
                parsedExpressions.Clear();
                parsedExpressions.Capacity = rawExpression.Length / avgParsedItemLength;

                parsedTypes.Clear();
                parsedTypes.Capacity = rawExpression.Length / avgParsedItemLength;

                rawExpression = value;
            }
        }

        public ExpressionParser() 
        {
            parsedExpressions = new List<string>();
            parsedTypes = new List<ParsedItemType>();

            rawExpression = string.Empty;
        }

        public ExpressionParser(string expression) : this()
        {
            StringExpression = expression;
        }

        public void ParseExpression()
        {
            ParsedItemType lastType = ParsedItemType.NotSet;

            parsedExpressions.Clear();
            parsedTypes.Clear();

            StringBuilder expBuffer = new StringBuilder(avgParsedItemLength);
            foreach (char c in rawExpression)
            {
                ParsedItemType currentType = getExpItemType(c);
                if(IsCoumpnoundable(lastType) && currentType != lastType)
                {
                    parseNewExpression(expBuffer, lastType);
                }

                lastType = currentType;
                if (IsCoumpnoundable(currentType))
                {
                    expBuffer.Append(c);
                    continue;
                }

                if (currentType == ParsedItemType.Invalid && SkipInvalidChars) { continue; }
                expBuffer.Append(c);

                parseNewExpression(expBuffer, currentType);
            }

            if (expBuffer.Length > 0)
            {
                parseNewExpression(expBuffer, lastType);
            }
        }
        private void parseNewExpression(StringBuilder charBuffer, ParsedItemType currentType)
        {
            string expression = (isTrashable(currentType)) ? string.Empty : charBuffer.ToString();

            parsedTypes.Add(currentType);
            parsedExpressions.Add(expression);
            charBuffer.Clear();
        }

        private bool IsCoumpnoundable(ParsedItemType type)
        {
            return (
                    type == ParsedItemType.Num ||
                    type == ParsedItemType.Name ||
                    type == ParsedItemType.WhiteSpace
                 );
        }

        private bool isTrashable(ParsedItemType type)
        {
            return (type == ParsedItemType.WhiteSpace);
        }

        private ParsedItemType getExpItemType(char c)
        {
            if (ParserHelper.IsNameChar(c))
            {
                return ParsedItemType.Name;
            }
            else if (ParserHelper.IsNum(c))
            {
                return ParsedItemType.Num;
            }
            else if (ParserHelper.IsWhiteSpace(c))
            {
                return ParsedItemType.WhiteSpace;
            }
            else if (ParserHelper.IsLeftBracket(c))
            {
                return ParsedItemType.LBracket;
            }
            else if (ParserHelper.IsRightBracket(c))
            {
                return ParsedItemType.RBracket;
            }
            else if (ParserHelper.IsOperator(c))
            {
                return ParsedItemType.Operator;
            }
            else if (ParserHelper.IsSeparator(c))
            {
                return ParsedItemType.Separator;
            }
            else
            {
                return ParsedItemType.Invalid; 
            }
        }
    }

    public enum ParsedItemType { Name, Num, LBracket, RBracket, Operator, Separator, WhiteSpace, Invalid, NotSet };

    public static class ParserHelper
    {
        public static bool IsNameChar(char a)
        {
            return (a == '_' || char.IsLetter(a));
        }

        private static char decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
        public static bool IsNum(char a)
        {
            return (a == decimalSeparator) || char.IsDigit(a);
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
