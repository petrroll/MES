﻿using System.Threading;
using System.Threading.Tasks;

namespace MESExamples.SocketServer
{
    public static class AsyncHelpers
    {
        public static Task GetCancelationTask(CancellationToken token)
        {
            var cancelationCompletionSource = new TaskCompletionSource<bool>();
            token.Register(() => cancelationCompletionSource.SetResult(true));

            return cancelationCompletionSource.Task;
        }
    }
}
