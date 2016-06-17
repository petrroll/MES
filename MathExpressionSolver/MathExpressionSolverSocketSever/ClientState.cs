using MathExpressionSolver.Builders;
using MathExpressionSolver.Controller;
using MathExpressionSolver.Parser;
using MathExpressionSolver.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MathExpressionSolverSocketSever
{
    class ClientState
    {
        CancellationToken ct;
        Controller<double> controller;

        public ClientState(CancellationToken token)
        {
            ct = token;
            controller = initController();
        }

        public async Task Talk(TcpClient client)
        {
            using (client)
            using (var clientStream = client.GetStream())
            using (var reader = new StreamReader(clientStream))
            using (var writer = new StreamWriter(clientStream))
            {
                await handleCommands(reader, writer);
            }

        }

        private async Task handleCommands(StreamReader reader, StreamWriter writer)
        {   
            Task lastWriteTask = Task.FromResult(0);
            var lastReadTask = reader.ReadLineAsync();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(60*60));

            while (!ct.IsCancellationRequested)
            {   
                var completedTask = await Task.WhenAny(timeoutTask, lastReadTask)
                              .ConfigureAwait(false);

                if (completedTask == timeoutTask) { break; }
                string command = lastReadTask.Result;
                if (command == "--bye") { break; }

                lastReadTask = reader.ReadLineAsync();
                timeoutTask = Task.Delay(TimeSpan.FromSeconds(15));

                string result = controller.ExecuteExpressionSafe(command).ToString();

                await lastWriteTask;
                await writer.FlushAsync();
                lastWriteTask = writer.WriteLineAsync(result).ContinueWith(async(_) => await writer.FlushAsync());
            }
        }

        private Controller<double> initController()
        {
            var parser = new ExpressionParser { SkipWhiteSpace = true };

            var customVariables = new Dictionary<string, double>();
            var customFunctions = new Dictionary<string, IFactorableCustFuncToken<double>>();

            ITokenFactory<double> factory = new DoubleTokenFactory { CustomVariables = customVariables, CustomFunctions = customFunctions };
            var tokenizer = new Tokenizer<double> { TokenFactory = factory };

            var treeBuilder = new ExpTreeBuilder<double>();

            return new Controller<double> { ExpTreeBuilder = treeBuilder, Parser = parser, Tokenizer = tokenizer };
        }
    }
}
