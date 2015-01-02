using System;

namespace MathExpressionSolver
{
    static class HelperExtensions
    {
        public static T[] SubArray<T>(this T[] data, int startingIndex, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, startingIndex, result, 0, length);
            return result;
        }

        public static T[] SubArray<T>(this T[] data, int startingIndex)
        {
            return SubArray(data, startingIndex, (data.Length - startingIndex));
        }
    }
}
