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
            T variableValue = ReturnResult(expression);

            if (Tokenizer.TokenFactory.CustomVariables == null) { throw new InvalidOperationException("Controller object not properly iniciazed."); }
            Tokenizer.TokenFactory.CustomVariables.Add(variableName, variableValue);

            return variableValue;
        }

        /// <summary>
        /// Creates an expression tree for a function and assign this newly created function to <see cref="CustomFunctions"/>.
        /// </summary>
        /// <param name="funcName">Custom function descriptor (name).</param>
        /// <param name="expression">An expression describing what the function will do.</param>
        /// <param name="argumentsNames">An array of argument descriptors (names).</param>
        /// <exception cref="InvalidOperationException">Some object has not been properly inicialized.</exception>
        /// <exception cref="TokenizerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        public void SaveFunction(string funcName, string expression, string[] argumentsNames)
        {
            if (!(Tokenizer.TokenFactory is IAdvancedTokenFactory<T>)) { throw new InvalidOperationException("Token factory doesn't support custom functions."); }
            IAdvancedTokenFactory<T> advTokenFactory = (IAdvancedTokenFactory<T>)Tokenizer.TokenFactory;

            IFactorableCustFuncToken<T> newFunction = new CustFuncToken<T>(argumentsNames.Length);

            advTokenFactory.ArgsArray = argumentsNames;
            advTokenFactory.CustomFunction = newFunction;

            newFunction.FuncTopToken = ReturnExpressionTopToken(expression);

            if (advTokenFactory.CustomFunctions == null) { throw new InvalidOperationException("Controller object not properly iniciazed."); }
            advTokenFactory.CustomFunctions.Add(funcName, newFunction);

            advTokenFactory.Clear();
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

            string exprRegex = @"\s*=\s*(?<expr>.*)";
            string nameRegex = @"(\p{Ll}|\p{Lu}|\p{Lt}|\p{Lo}|\p{Lm}|_)+";
            Match custVar = Regex.Match(expression, @"^(?<varName>" + nameRegex + @")" + exprRegex);

            string argumentsRegex = @"((?<argName>" + nameRegex + @")(;)?|(;\s)?)+";
            Match custFunc = Regex.Match(expression, @"^(?<funcName>" + nameRegex + @")\(" + argumentsRegex + @"\)" + exprRegex);

            if (custVar.Success)
            {
                string variableName = custVar.Groups["varName"].Value;
                string variableExpression = custVar.Groups["expr"].Value;

                string variableValue = SaveVariable(variableName, variableExpression).ToString();
                return variableName + " = " + variableValue;
            }
            else if (custFunc.Success)
            {
                string funcName = custFunc.Groups["funcName"].Value;
                string funcExpression = custFunc.Groups["expr"].Value;
                string[] argumentsNames = custFunc.Groups["argName"].Captures.ToArray();

                SaveFunction(funcName, funcExpression, argumentsNames);
                return "Function " + funcName + " set.";
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
