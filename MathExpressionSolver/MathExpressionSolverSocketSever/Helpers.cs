using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MathExpressionSolverSocketSever
{
    public static class AsyncHelpers
    {
        public static Task GetCancelationTask(CancellationToken token)
        {
            TaskCompletionSource<bool> cancelationCompletionSource = new TaskCompletionSource<bool>();
            token.Register(() => cancelationCompletionSource.SetResult(true));

            return cancelationCompletionSource.Task;
        }
    }
}
