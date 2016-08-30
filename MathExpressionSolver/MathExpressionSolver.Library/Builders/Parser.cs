using MathExpressionSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathExpressionSolver.Parser
{

    public class ExpressionParser : IParser
    {
        const int _avgParsedItemLength = 4;

        /// <summary>
        /// Is whitespace to be skipped or included in result. Implicitly false.
        /// </summary>
        public bool SkipWhiteSpace { get; set; } = true;

        public ParsedItem[] ParseExpression(string rawExpression)
        {
            if (rawExpression == null) { throw new ArgumentNullException(nameof(rawExpression), "StringExpression null"); }

            var expBuffer = new StringBuilder(_avgParsedItemLength);
            var parsedItems = new List<ParsedItem>(rawExpression.Length / _avgParsedItemLength);

            ParsedItemType lastType = ParsedItemType.NotSet;
            foreach (char c in rawExpression)
            {
                ParsedItemType currentType = getExpItemType(c);
                if (isCoumpnoundable(lastType) && currentType != lastType) { parseNewExpression(expBuffer, lastType, parsedItems); }

                lastType = currentType;
                expBuffer.Append(c);

                if (!isCoumpnoundable(currentType)) { parseNewExpression(expBuffer, currentType, parsedItems); }
            }
            flushBuffer(expBuffer, lastType, parsedItems);

            return parsedItems.ToArray();
        }

        private void flushBuffer(StringBuilder expBuffer, ParsedItemType lastType, List<ParsedItem> parsedItems)
        {
            if (expBuffer.Length > 0)
            {
                parseNewExpression(expBuffer, lastType, parsedItems);
            }
        }

        private void parseNewExpression(StringBuilder expBuffer, ParsedItemType currentType, List<ParsedItem> parsedItems)
        {
            string expression = expBuffer.ToString();
            expBuffer.Clear();

            if (isSkipable(currentType)) { return; }

            parsedItems.Add(new ParsedItem(expression, currentType));

        }

        private bool isCoumpnoundable(ParsedItemType type)
        {
            return (
                    type == ParsedItemType.Value ||
                    type == ParsedItemType.Name ||
                    type == ParsedItemType.WhiteSpace
                 );
        }

        private bool isSkipable(ParsedItemType type)
        {
            return (type == ParsedItemType.WhiteSpace && SkipWhiteSpace);
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
            else if (ParserHelper.IsSeparator(c))
            {
                return ParsedItemType.Separator;
            }
            else if (ParserHelper.IsOperator(c))
            {
                return ParsedItemType.Operator;
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
        public string Value { get; private set; }
        public ParsedItemType Type { get; private set; }

        public ParsedItem(string value, ParsedItemType type)
        {
            this.Value = value;
            this.Type = type;
        }

        public override string ToString()
        {
            return $"[{this.Value}:{this.Type}]";
        }
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
        //Ascii codes taken from: http://nemesis.lonestar.org/reference/telecom/codes/ascii.html
        public static bool IsNameChar(char a)
        {
            return (a == '_') || (a >= 'a' && a <= 'z') || (a >= 'A' && a <= 'Z');
        }

        public static bool IsNum(char a)
        {
            return (a == '.') || (a >= '0' && a <= '9');
        }


        public static bool IsLeftBracket(char a)
        {
            return (a == '(') || (a == '[') || (a == '{');
        }

        public static bool IsRightBracket(char a)
        {
            return (a == ')') || (a == ']') || (a == '}');
        }

        public static bool IsOperator(char a)
        {
            return (a == '!') || (a >= '#' && a <= '&') || (a >= '*' && a <= '/') || (a >= ':' && a <= '?') || (a == '\\') || (a == '^') || (a == '|') || (a == '~');
        }

        public static bool IsSeparator(char a)
        {
            return (a == ';' || a == ','); 
        }

        public static bool IsWhiteSpace(char a)
        {
            return char.IsWhiteSpace(a);
        }
    }
}
