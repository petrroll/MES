using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MathExpressionSolver.Parser
{
    class ExpressionParser
    {
        const int avgExpressionItemLength = 4;
        public bool SkipInvalidChars { get; set; } = false;


        private List<string> parsedExpressions;
        private List<ExpressionItemType> parsedTypes;

        public string[] ParsedExpression { get { return parsedExpressions.ToArray(); } }
        public ExpressionItemType[] ParsedTypes { get { return parsedTypes.ToArray(); } }

        private string rawExpression;

        public string StringExpression
        {
            set
            {
                parsedExpressions.Clear();
                parsedExpressions.Capacity = rawExpression.Length / avgExpressionItemLength;

                parsedTypes.Clear();
                parsedTypes.Capacity = rawExpression.Length / avgExpressionItemLength;

                rawExpression = value;
            }
        }

        public ExpressionParser() 
        {
            parsedExpressions = new List<string>();
            parsedTypes = new List<ExpressionItemType>();

            rawExpression = string.Empty;
        }

        public ExpressionParser(string expression) : this()
        {
            StringExpression = expression;
        }

        public void ParseExpression()
        {
            ExpressionItemType lastType = ExpressionItemType.NotSet;

            parsedExpressions.Clear();
            parsedTypes.Clear();

            StringBuilder expBuffer = new StringBuilder(avgExpressionItemLength);
            foreach (char c in rawExpression)
            {
                ExpressionItemType currentType = getExpItemType(c);
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

                if (currentType == ExpressionItemType.Invalid && SkipInvalidChars) { continue; }
                expBuffer.Append(c);

                parseNewExpression(expBuffer, currentType);
            }

            if (expBuffer.Length > 0)
            {
                parseNewExpression(expBuffer, lastType);
            }
        }
        private void parseNewExpression(StringBuilder charBuffer, ExpressionItemType currentType)
        {
            string expression = (isTrashable(currentType)) ? string.Empty : charBuffer.ToString();

            parsedTypes.Add(currentType);
            parsedExpressions.Add(expression);
            charBuffer.Clear();
        }

        private bool IsCoumpnoundable(ExpressionItemType type)
        {
            return (
                    type == ExpressionItemType.Num ||
                    type == ExpressionItemType.Name ||
                    type == ExpressionItemType.WhiteSpace
                 );
        }

        private bool isTrashable(ExpressionItemType type)
        {
            return (type == ExpressionItemType.WhiteSpace);
        }

        private ExpressionItemType getExpItemType(char c)
        {
            if (ParserHelper.IsNameChar(c))
            {
                return ExpressionItemType.Name;
            }
            else if (ParserHelper.IsNum(c))
            {
                return ExpressionItemType.Num;
            }
            else if (ParserHelper.IsWhiteSpace(c))
            {
                return ExpressionItemType.WhiteSpace;
            }
            else if (ParserHelper.IsLeftBracket(c))
            {
                return ExpressionItemType.LBracket;
            }
            else if (ParserHelper.IsRightBracket(c))
            {
                return ExpressionItemType.RBracket;
            }
            else if (ParserHelper.IsOperator(c))
            {
                return ExpressionItemType.Operator;
            }
            else if (ParserHelper.IsSeparator(c))
            {
                return ExpressionItemType.Separator;
            }
            else
            {
                return ExpressionItemType.Invalid; 
            }
        }
    }

    public enum ExpressionItemType { Name, Num, LBracket, RBracket, Operator, Separator, WhiteSpace, Invalid, NotSet };

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
