using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MathExpressionSolver.Controller
{
    /// <summary>
    /// Executes expressions and handles custom variables and functions assigment.
    /// </summary>
    /// <typeparam name="T">Token base type.</typeparam>
    class Controller<T>
    {
        /// <summary>
        /// Is used for expression parsing from <see cref="string"/> to <see cref="ParsedItem[]"/>.
        /// </summary>
        public ExpressionParser Parser { private get; set; }
        /// <summary>
        /// Is used for converting <see cref="ParsedItem[]"/> to <see cref="IFactorableToken{T}[]"/>.
        /// </summary>
        public Tokenizer<T> Tokenizer { private get; set; }
        /// <summary>
        /// Is used for building an expression tree out of <see cref="IFactorableToken{T}[]"/>.
        /// </summary>
        public ExpTreeBuilder<T> ExpTreeBuilder { private get; set; }

        /// <summary>
        /// Dictionary with custom-defined variables usable in expressions, should be the same object as in <see cref="ITokenFactory{T}"/>.
        /// </summary>
        public Dictionary<string, T> CustomVariables { get; set; }



        /// <summary>
        /// Returns a top <see cref="IToken{T}"/> of a expression tree to specified <see cref="string"/> expression.
        /// </summary>
        /// <param name="expression">Expression whose top <see cref="IToken{T}"/> is wanted.</param>
        /// <exception cref="InvalidOperationException">Some object has not been properly inicialized.</exception>
        /// <exception cref="TokenizerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        /// <returns>Top <see cref="IToken{T}"/> of specified expression.</returns>
        public IToken<T> ReturnExpressionTopToken(string expression)
        {
            testInicialization();

            Parser.StringExpression = expression;
            Parser.ParseExpression();

            Tokenizer.DataToBeTokenized = Parser.ParsedItems;
            Tokenizer.Tokenize();

            ExpTreeBuilder.RawTokens = Tokenizer.Tokens;
            ExpTreeBuilder.CreateExpressionTree();

            IToken<T> treeTop = ExpTreeBuilder.TreeTop;
            return treeTop;
        }

        /// <summary>
        /// Returns a result of a specified <see cref="string"/> expression.
        /// </summary>
        /// <param name="expression">Expression whose result is wanted.</param>
        /// <exception cref="InvalidOperationException">Some object has not been properly inicialized.</exception>
        /// <exception cref="TokenizerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        /// <returns>Result of specified expression.</returns>
        public T ReturnResult(string expression)
        {
            IToken<T> treeTop = ReturnExpressionTopToken(expression);
            return treeTop.ReturnValue();
        }

        /// <summary>
        /// Computes a result of a given expression and saves it to <see cref="CustomVariables"/>.
        /// </summary>
        /// <param name="variableName">Custom variable name.</param>
        /// <param name="expression">An expression whose result is to be set as a value of the variable.</param>
        /// <exception cref="InvalidOperationException">Some object has not been properly inicialized.</exception>
        /// <exception cref="TokenizerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        /// <returns>String in a form of "variableName = variableValue".</returns>
        public T SaveVariable(string variableName, string expression)
        {
            if(CustomVariables == null) { throw new InvalidOperationException("Controller object not properly iniciazed."); }

            T variableValue = ReturnResult(expression);
            CustomVariables.Add(variableName, variableValue);
            return variableValue;
        }

        /// <summary>
        /// Executes a expression command (compute an expression, assign variable, assign custom function) and returns its result.
        /// </summary>
        /// <param name="expression">An expression to be executed.</param>
        /// <exception cref="InvalidOperationException">Some object has not been properly inicialized.</exception>
        /// <exception cref="TokenizerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        /// <returns>String with information about expression result.</returns>
        public string ExecuteExpression(string expression)
        {
            string nameRegex = @"(\p{Ll}|\p{Lu}|\p{Lt}|\p{Lo}|\p{Lm}|_)+";

            if (Regex.IsMatch(expression, @"^" + nameRegex + @"\s*="))
            {
                Match exprMatch = Regex.Match(expression, @"^(?<varName>" + nameRegex + @")\s*=\s*(?<varExpr>.*)");

                string variableName = exprMatch.Groups["varName"].Value;
                string variableExpression = exprMatch.Groups["varExpr"].Value;

                string variableValue = SaveVariable(variableName, variableExpression).ToString();
                return variableName + " = " + variableValue;
            }
            else
            {
                return ReturnResult(expression).ToString();
            }
        }

        /// <summary>
        /// Executes a expression command (compute an expression, assign variable, assign custom function) and returns its result.
        /// </summary>
        /// <param name="expression">An expression to be executed</param>
        /// <returns>String with information about expression result.</returns>
        public string ExecuteExpressionSafe(string expression)
        {
            try { return ExecuteExpression(expression); }
            catch(ExpressionException ex)
            {
               return "Error: " + ex.Message;
            }
            catch(Exception ex)
            {
                return "Expression unrelated exception: " + ex.Message;
            }
        }

        private void testInicialization()
        {
            if(Parser == null ||
                Tokenizer == null ||
                ExpTreeBuilder == null)
            {
                throw new InvalidOperationException("Controller object not properly iniciazed.");
            }
        }
    }
}
