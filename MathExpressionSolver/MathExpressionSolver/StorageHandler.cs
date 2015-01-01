using MathExpressionSolver.Tokens;
using System.Collections.Generic;


namespace MathExpressionSolver
{
    class StorageHandler<T>
    {
        public Dictionary<string, T> Variables { get; private set; } = new Dictionary<string, T>();
        public Dictionary<string, IToken<T>> Functions { get; private set; } = new Dictionary<string, IToken<T>>();

        public void AddVariable(string variableName, T value)
        {
            Variables[variableName] = value;
        }

        public void AddFunction (string funcName, IToken<T> value)
        {
            Functions[funcName] = value;
        }
    }
}
