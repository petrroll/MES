using System;
using System.Collections.Generic;

namespace MathExpressionSolver
{
    static class HelperExtensions
    {

        /// <summary>
        /// Returns a subarray starting at specified index of a specified length.
        /// </summary>
        /// <param name="startingIndex">Index of first item that is going to be in a subarray.</param>
        /// <param name="length">Number of items that are going to be in a subarray.</param>
        /// <returns>A copy of the original array starting at specified index of specified length.</returns>
        public static T[] SubArray<T>(this T[] data, int startingIndex, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, startingIndex, result, 0, length);
            return result;
        }

        /// <summary>
        /// Returns a subarray starting at specified index and ending at the last index of the original array.
        /// </summary>
        /// <param name="startingIndex">Index of first item that is going to be in a subarray.</param>
        /// <returns>A copy of the original array starting at specified index.</returns>
        public static T[] SubArray<T>(this T[] data, int startingIndex)
        {
            return SubArray(data, startingIndex, (data.Length - startingIndex));
        }

        /// <summary>
        /// Determines whether <see cref="Stack{T}"/> is empty.
        /// </summary>
        /// <returns>True when stack is empty; false if it's not.</returns>
        public static bool IsEmpty<T>(this Stack<T> stack)
        {
            return (stack.Count == 0);
        }
    }


    [Serializable]
    public class ExpressionException : Exception
    {
        public ExpressionException() { }
        public ExpressionException(string message) : base(message) { }
        public ExpressionException(string message, Exception inner) : base(message, inner) { }
        protected ExpressionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
