using System;
using System.Text.RegularExpressions;
using MathExpressionSolver.Tokens;
using MathExpressionSolver.Interfaces;

namespace MathExpressionSolver.Controller
{
    /// <summary>
    /// Executes expressions and handles custom variables and functions assigment.
    /// </summary>
    /// <typeparam name="T">Token base type.</typeparam>
    public class Controller<T>
    {
        /// <summary>
        /// Is used for expression parsing from <see cref="string"/> to <see cref="ParsedItem[]"/>.
        /// </summary>
        public IParser Parser { private get; set; }
        /// <summary>
        /// Is used for converting <see cref="ParsedItem[]"/> to <see cref="IFactorableToken{T}[]"/>.
        /// </summary>
        public ITokenizer<T> Tokenizer { private get; set; }
        /// <summary>
        /// Is used for building an expression tree out of <see cref="IFactorableToken{T}[]"/>.
        /// </summary>
        public IExpTreeBuilder<T> ExpTreeBuilder { private get; set; }

        /// <summary>
        /// Returns a top <see cref="IToken{T}"/> of a expression tree to specified <see cref="string"/> expression.
        /// </summary>
        /// <param name="expression">Expression whose top <see cref="IToken{T}"/> is wanted.</param>
        /// <exception cref="InvalidOperationException">Some object has not been properly inicialized.</exception>
        /// <exception cref="ControllerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        /// <returns>Top <see cref="IToken{T}"/> of specified expression.</returns>
        public IToken<T> ReturnExpressionTopToken(string expression)
        {
            testInicialization();

            var parsedItems = Parser.ParseExpression(expression);
            var tokens = Tokenizer.Tokenize(parsedItems);
            var treeTop = ExpTreeBuilder.CreateExpressionTree(tokens);

            return treeTop;
        }

        /// <summary>
        /// Returns a result of a specified <see cref="string"/> expression.
        /// </summary>
        /// <param name="expression">Expression whose result is wanted.</param>
        /// <exception cref="InvalidOperationException">Some object has not been properly inicialized.</exception>
        /// <exception cref="ControllerException">Expression is empty.</exception>
        /// <exception cref="TokenizerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        /// <returns>Result of specified expression.</returns>
        public T ReturnResult(string expression)
        {
            IToken<T> treeTop = ReturnExpressionTopToken(expression);
            if(treeTop == null) { throw new ControllerException("Expression is empty."); }
            return treeTop.ReturnValue();
        }

        /// <summary>
        /// Computes a result of a given expression and saves it to custom variables in TokenFactory.
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
            Tokenizer.TokenFactory.CustomVariables[variableName] = variableValue;

            return variableValue;
        }

        /// <summary>
        /// Creates an expression tree for a function and assign this newly created function to custom functions in TokenFactory.
        /// </summary>
        /// <param name="funcName">Custom function descriptor (name).</param>
        /// <param name="expression">An expression describing what the function will do.</param>
        /// <param name="argumentsNames">An array of argument descriptors (names).</param>
        /// <exception cref="InvalidOperationException">Some object has not been properly inicialized.</exception>
        /// <exception cref="TokenizerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        public void SaveFunction(string funcName, string expression, string[] argumentsNames)
        {
            if (Tokenizer.TokenFactory == null) { throw new InvalidOperationException("Controller object not properly iniciazed."); }
            if (!(Tokenizer.TokenFactory is ICustomFunctionsAwareTokenFactory<T>)) { throw new InvalidOperationException("Token factory doesn't support custom functions."); }
            var advTokenFactory = (ICustomFunctionsAwareTokenFactory<T>)Tokenizer.TokenFactory;

            IFactorableCustFuncToken<T> newFunction = new CustFuncToken<T>(argumentsNames.Length);

            advTokenFactory.ArgsArray = argumentsNames;
            advTokenFactory.CustomFunction = newFunction;

            newFunction.MutFuncTopToken = ReturnExpressionTopToken(expression);
            if(newFunction.FuncTopToken == null) { throw new ControllerException("Custom function body is empty."); }

            if (advTokenFactory.CustomFunctions == null) { throw new InvalidOperationException("Controller object not properly iniciazed."); }
            advTokenFactory.CustomFunctions[funcName] = newFunction;

            advTokenFactory.Clear();
        }

        /// <summary>
        /// Executes a expression command (compute an expression, assign variable, assign custom function) and returns its result.
        /// </summary>
        /// <param name="expression">An expression to be executed.</param>
        /// <exception cref="InvalidOperationException">Some object has not been properly inicialized.</exception>
        /// <exception cref="TokenizerException">Expression can't be properly tokenized.</exception>
        /// <exception cref="ExpTreeBuilderException">Expression tree can't be build.</exception>
        /// <exception cref="ControllerException">Invalid expression.</exception>
        /// <returns>A struct containing all information about result.</returns>
        public Result<T> ExecuteExpression(string expression)
        {
            string possVariableRegex = @"^[^\(\)]+=.*$";
            string possFuncRegex = @"^.+\(.+=.*$";

            string exprRegex = @"\s*=\s*(?<expr>.*)";
            string nameRegex = @"(\p{Ll}|\p{Lu}|\p{Lt}|\p{Lo}|\p{Lm}|_)+";

            if (Regex.IsMatch(expression, possVariableRegex))
            {
                Match custVar = Regex.Match(expression, @"^(?<varName>" + nameRegex + @")" + exprRegex + "$");
                if (!custVar.Success) { throw new ControllerException("Given expression is not a valid variable assigment."); }

                string variableName = custVar.Groups["varName"].Value;
                string variableExpression = custVar.Groups["expr"].Value;

                T variableValue = SaveVariable(variableName, variableExpression);
                return new Result<T>(ExpressionType.NewVariable, variableValue, additionalData: variableName);
            }
            else if (Regex.IsMatch(expression, possFuncRegex))
            {
                string argumentsRegex = @"((?<argName>" + nameRegex + @")(;)?|(;\s)?)+";
                Match custFunc = Regex.Match(expression, @"^(?<funcName>" + nameRegex + @")\(" + argumentsRegex + @"\)" + exprRegex + "$");
                if(!custFunc.Success) { throw new ControllerException("Given expression is not a valid custom function assigment."); }

                string funcName = custFunc.Groups["funcName"].Value;
                string funcExpression = custFunc.Groups["expr"].Value;
                string[] argumentsNames = custFunc.Groups["argName"].Captures.ToArray();

                SaveFunction(funcName, funcExpression, argumentsNames);
                return new Result<T>(ExpressionType.NewFunction, default(T), additionalData: funcName);
            }
            else
            {
                T result = ReturnResult(expression);
                return new Result<T>(ExpressionType.Evaluation, result);
            }
        }

        /// <summary>
        /// Executes a expression command (compute an expression, assign variable, assign custom function) and returns its result.
        /// </summary>
        /// <param name="expression">An expression to be executed</param>
        /// <returns>Struct containing all information about result.</returns>
        public Result<T> ExecuteExpressionSafe(string expression)
        {
            try { return ExecuteExpression(expression); }
            catch(ExpressionException ex)
            {
                return new Result<T>(ex);
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

    public enum ExpressionType { Evaluation, NewFunction, NewVariable }
    /// <summary>
    /// Struct encapsulating result from ExpressionEvaluator.
    /// 
    /// If Exception is empty then the operation succeeded. If it's non-empty then it failed.
    /// </summary>
    /// <typeparam name="T">Type of results.</typeparam>
    public struct Result<T>
    {
        public Exception Exception { get; private set; }
        public ExpressionType Type { get; private set; }   
        public T Value { get; private set; }
        public string AdditionalData { get; private set; } //E.g. the name of new function or a variable

        public Result(ExpressionType expressionType, T value, string additionalData = null, Exception ex = null)
        {
            Exception = ex;
            Value = value;
            Type = expressionType;
            AdditionalData = additionalData;
        }

        public Result(Exception ex)
        {
            Exception = ex;
            Value = default(T);
            Type = default(ExpressionType);
            AdditionalData = null;
        }

        public override string ToString()
        {
            if (Exception != null)
            {
                return $"Error: {Exception.Message}";
            }

            switch (Type)
            {
                case ExpressionType.Evaluation:
                    return Value.ToString();
                case ExpressionType.NewFunction:
                    return $"Function {AdditionalData} set.";
                case ExpressionType.NewVariable:
                    return $"{AdditionalData} = {Value}";
                default:
                    throw new InvalidOperationException("Unimplemented type.");
            }
        }
    }

    public class ControllerException : ExpressionException
    {
        public ControllerException() { }
        public ControllerException(string message) : base(message) { }
        public ControllerException(string message, System.Exception inner) : base(message, inner) { }
    }
}
