using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionSolver
{
    static class HelperExtensions
    {
        public static T[] SubArray<T>(this T[] data, int startingIndex, int endingIndex)
        {
            int length = (endingIndex - startingIndex) + 1;
            T[] result = new T[length];
            Array.Copy(data, startingIndex, result, 0, length);
            return result;
        }
    }
}
