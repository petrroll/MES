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
        private const int timeoutSec = 60 * 60;
        bool _log;

        CancellationToken ct;
        Controller<double> controller;

        public ClientState(CancellationToken token, bool log)
        {
            _log = log;
            ct = token;
            controller = initController();
        }

        public async Task Talk(TcpClient client)
        {
            try
            {
                using (client)
                using (var clientStream = client.GetStream())
                using (var reader = new StreamReader(clientStream))
                using (var writer = new StreamWriter(clientStream) { AutoFlush = true })
                {
                    await handleCommands(reader, writer);
                }

            }
            catch (SocketException ex)
            {
                log($"Client network error (code: {ex.ErrorCode}): {ex.Message}");
            }
            catch (IOException ex)
            {
                log($"Client network error: {ex.Message}");
            }

            log("Client stopped talking.");
        }

        private async Task handleCommands(StreamReader reader, StreamWriter writer)
        {   
            Task lastWriteTask = Task.FromResult(0);
            var lastReadTask = reader.ReadLineAsync();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSec));

            while (!ct.IsCancellationRequested)
            {   
                var completedTask = await Task.WhenAny(timeoutTask, lastReadTask)
                              .ConfigureAwait(false);

                if (completedTask == timeoutTask) { break; }
                string command = lastReadTask.Result;
                if (command == "--bye") { log("Client bye"); break; }

                lastReadTask = reader.ReadLineAsync();
                timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSec));

                string result = controller.ExecuteExpressionSafe(command).ToString();

                await lastWriteTask;
                lastWriteTask = writer.WriteLineAsync(result);
            }
        }

        private void log(string message)
        {
            if (_log) { Console.WriteLine(message); }
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
