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


        private List<string> parsedExpressions;
        private List<TokenType> parsedTypes;

        public string[] ParsedExpression { get { return parsedExpressions.ToArray(); } }
        public TokenType[] ParsedTypes { get { return parsedTypes.ToArray(); } }

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
            parsedExpressions = new List<string>();
            parsedTypes = new List<TokenType>();

            rawExpression = string.Empty;
        }

        public ExpressionParser(string expression) : this()
        {
            StringExpression = expression;
        }

        public void ParseExpression()
        {
            TokenType lastType = TokenType.NotSet;

            parsedExpressions.Clear();
            parsedTypes.Clear();

            StringBuilder charBuffer = new StringBuilder(avgTokenLength);
            foreach (char token in rawExpression)
            {
                TokenType currentType = getTokenType(token);
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

                if (currentType == TokenType.Invalid && SkipInvalidChars) { continue; }
                charBuffer.Append(token);

                parseNewExpression(charBuffer, currentType);
            }

            if (charBuffer.Length > 0)
            {
                parseNewExpression(charBuffer, lastType);
            }
        }
        private void parseNewExpression(StringBuilder charBuffer, TokenType currentType)
        {
            string expression = (isTrashable(currentType)) ? string.Empty : charBuffer.ToString();

            parsedTypes.Add(currentType);
            parsedExpressions.Add(expression);
            charBuffer.Clear();
        }

        private bool IsCoumpnoundable(TokenType type)
        {
            return (
                    type == TokenType.Num ||
                    type == TokenType.Name ||
                    type == TokenType.WhiteSpace
                 );
        }

        private bool isTrashable(TokenType type)
        {
            return (type == TokenType.WhiteSpace);
        }

        private TokenType getTokenType(char token)
        {
            if (ParserHelper.IsNameChar(token))
            {
                return TokenType.Name;
            }
            else if (ParserHelper.IsNum(token))
            {
                return TokenType.Num;
            }
            else if (ParserHelper.IsWhiteSpace(token))
            {
                return TokenType.WhiteSpace;
            }
            else if (ParserHelper.IsLeftBracket(token))
            {
                return TokenType.LBracket;
            }
            else if (ParserHelper.IsRightBracket(token))
            {
                return TokenType.RBracket;
            }
            else if (ParserHelper.IsOperator(token))
            {
                return TokenType.Operator;
            }
            else if (ParserHelper.IsSeparator(token))
            {
                return TokenType.Separator;
            }
            else
            {
                return TokenType.Invalid; 
            }
        }
    }

    public enum TokenType { Name, Num, LBracket, RBracket, Operator, Separator, WhiteSpace, Invalid, NotSet };

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
