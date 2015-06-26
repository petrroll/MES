using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MathExpressionSolver.Parser
{
    /// <summary>
    /// Divides expression string into an array of substrings and their types.
    /// </summary>
    class ExpressionParser
    {
        const int avgParsedItemLength = 4;

        /// <summary>
        /// Are invalid characters to be skipped or included in result. Implicitly false.
        /// </summary>
        /// <remarks>
        /// An invalid character is a character that doesn't fall into any category.
        /// </remarks>
        public bool SkipInvalidChars { get; set; } = false;
        /// <summary>
        /// Is whitespace to be skipped or included in result. Implicitly false.
        /// </summary>
        public bool SkipWhiteSpace { get; set; } = true;

        private List<ParsedItem> parsedItems;
        /// <summary>
        /// Parsed <see cref="StringExpression"/> (after <see cref="ParseExpression"/> is called).
        /// </summary>
        public ParsedItem[] ParsedItems { get { return parsedItems.ToArray(); } }

        private string rawExpression;

        /// <summary>
        /// Expression string that is to be parsed.
        /// </summary>
        public string StringExpression
        {
            set
            {
                Clear();

                rawExpression = value;
                parsedItems.Capacity = rawExpression.Length / avgParsedItemLength;

            }
        }

        public ExpressionParser()
        {
            parsedItems = new List<ParsedItem>();
            rawExpression = string.Empty;
            Clear();
        }

        /// <summary>
        /// Automacitally sets <see cref="StringExpression"/> property.
        /// </summary>
        /// <param name="expression">An expression string to be parsed</param>
        public ExpressionParser(string expression) : this()
        {
            StringExpression = expression;
        }

        /// <summary>
        /// Clears <see cref="ParsedItems"/> and resets <see cref="ExpressionParser"/> state.
        /// </summary>
        public void Clear()
        {
            parsedItems.Clear();
        }

        /// <summary>
        /// Parses the expression and appends output to a corresponding variable.
        /// </summary>
        /// <remarks>
        /// Parses the expression set in <see cref="StringExpression"/> and saves the result to <see cref="ParsedItems"/>.
        /// </remarks>
        public void ParseExpression()
        {
            ParsedItemType lastType = ParsedItemType.NotSet;

            StringBuilder expBuffer = new StringBuilder(avgParsedItemLength);
            foreach (char c in rawExpression)
            {
                ParsedItemType currentType = getExpItemType(c);
                if (IsCoumpnoundable(lastType) && currentType != lastType)
                {
                    parseNewExpression(expBuffer, lastType);
                }

                lastType = currentType;
                if (IsCoumpnoundable(currentType))
                {
                    expBuffer.Append(c);
                    continue;
                }

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
            charBuffer.Clear();

            if (isSkipable(currentType)) { return; }

            parsedItems.Add(new ParsedItem(expression, currentType));

        }

        private bool IsCoumpnoundable(ParsedItemType type)
        {
            return (
                    type == ParsedItemType.Value ||
                    type == ParsedItemType.Name ||
                    type == ParsedItemType.WhiteSpace
                 );
        }

        private bool isTrashable(ParsedItemType type)
        {
            return (type == ParsedItemType.WhiteSpace);
        }

        private bool isSkipable(ParsedItemType type)
        {
            return ((type == ParsedItemType.Invalid && SkipInvalidChars) ||
                (type == ParsedItemType.WhiteSpace && SkipWhiteSpace));
        }

        private ParsedItemType getExpItemType(char c)
        {
            if (ParserHelper.IsNameChar(c))
            {
                return ParsedItemType.Name;
            }
            else if (ParserHelper.IsNum(c))
            {
                return ParsedItemType.Value;
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

    /// <summary>
    /// Container for the string literal and its type (<see cref="ParsedItemType"/>) according to Parser clasification.
    /// </summary>
    public struct ParsedItem
    {
        public ParsedItem(string value, ParsedItemType type)
        {
            this.value = value;
            this.type = type;
        }

        public override string ToString()
        {
            return Value;
        }

        private string value;
        private ParsedItemType type;

        public string Value { get { return value; } }
        public ParsedItemType Type { get { return type; } }
    }

    /// <summary>
    /// Types of substrings that Parser understands.
    /// </summary>
    /// <remarks>
    /// See <see cref="ParserHelper"/> for more information about specific types.
    /// </remarks>
    public enum ParsedItemType { Name, Value, LBracket, RBracket, Operator, Separator, WhiteSpace, Invalid, NotSet };

    /// <summary>
    /// Determines what type of substring a specific char is.
    /// </summary>
    /// <remarks>
    /// See <see cref="ParsedItemType"/> for more information about Parser substring types.
    /// </remarks>
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
                case '>':
                    return true;
                case '<':
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
