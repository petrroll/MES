using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MathExpressionSolver.Parser
{
    class ExpressionParser
    {
        const int avgTokenLength = 4;
        public bool SkipInvalidChars { get; set; } = false;


        private List<string> parsedExpression;
        private List<ParsedSubstringType> parsedTypes;

        public string[] ParsedExpression { get { return parsedExpression.ToArray(); } }
        public ParsedSubstringType[] ParsedTypes { get { return parsedTypes.ToArray(); } }

        private string rawExpression;

        public string StringExpression
        {
            set
            {
                parsedExpression.Clear();
                parsedExpression.Capacity = rawExpression.Length / avgTokenLength;

                parsedTypes.Clear();
                parsedTypes.Capacity = rawExpression.Length / avgTokenLength;

                rawExpression = value;
            }
        }

        public ExpressionParser() 
        {
            parsedExpression = new List<string>();
            parsedTypes = new List<ParsedSubstringType>();

            rawExpression = string.Empty;
        }

        public ExpressionParser(string expression) : this()
        {
            StringExpression = expression;
        }

        public void ParseExpression()
        {
            ParsedSubstringType lastType = ParsedSubstringType.NotSet;

            parsedExpression.Clear();
            parsedTypes.Clear();

            StringBuilder charBuffer = new StringBuilder(avgTokenLength);
            foreach (char token in rawExpression)
            {
                ParsedSubstringType currentType = getTokenType(token);
                if(IsCoumpnoundable(lastType) && currentType != lastType)
                {
                    parseNewExpression(charBuffer, lastType);
                }

                lastType = currentType;
                if (IsCoumpnoundable(currentType))
                {
                    charBuffer.Append(token);
                    continue;
                }

                if (currentType == ParsedSubstringType.Invalid && SkipInvalidChars) { continue; }
                charBuffer.Append(token);

                parseNewExpression(charBuffer, currentType);
            }

            if (charBuffer.Length > 0)
            {
                parseNewExpression(charBuffer, lastType);
            }
        }
        private void parseNewExpression(StringBuilder charBuffer, ParsedSubstringType currentType)
        {
            string expression = (isTrashable(currentType)) ? string.Empty : charBuffer.ToString();

            parsedTypes.Add(currentType);
            parsedExpression.Add(expression);
            charBuffer.Clear();
        }

        private bool IsCoumpnoundable(ParsedSubstringType type)
        {
            return (
                    type == ParsedSubstringType.Num ||
                    type == ParsedSubstringType.Name ||
                    type == ParsedSubstringType.WhiteSpace
                 );
        }

        private bool isTrashable(ParsedSubstringType type)
        {
            return (type == ParsedSubstringType.WhiteSpace);
        }

        private ParsedSubstringType getTokenType(char token)
        {
            if (ParserHelper.IsNameChar(token))
            {
                return ParsedSubstringType.Name;
            }
            else if (ParserHelper.IsNum(token))
            {
                return ParsedSubstringType.Num;
            }
            else if (ParserHelper.IsWhiteSpace(token))
            {
                return ParsedSubstringType.WhiteSpace;
            }
            else if (ParserHelper.IsLeftBracket(token))
            {
                return ParsedSubstringType.LBracket;
            }
            else if (ParserHelper.IsRightBracket(token))
            {
                return ParsedSubstringType.RBracket;
            }
            else if (ParserHelper.IsOperator(token))
            {
                return ParsedSubstringType.Operator;
            }
            else if (ParserHelper.IsSeparator(token))
            {
                return ParsedSubstringType.Separator;
            }
            else
            {
                return ParsedSubstringType.Invalid; 
            }
        }
    }

    public enum ParsedSubstringType { Name, Num, LBracket, RBracket, Operator, Separator, WhiteSpace, Invalid, NotSet };

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
