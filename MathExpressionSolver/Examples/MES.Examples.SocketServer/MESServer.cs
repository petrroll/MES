using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
namespace MESExamples.SocketServer
{
    class MESServer
    {
        private const int _port = 4586;
        readonly bool _log;

        CancellationTokenSource cancelationSource;
        Task cancelationTask;

        TcpListener serverListener;

        public MESServer(bool log)
        {
            _log = log;
            cancelationSource = new CancellationTokenSource();
            cancelationTask = AsyncHelpers.GetCancelationTask(cancelationSource.Token);

            serverListener = new TcpListener(IPAddress.Any, _port);
        }

        async public Task Work()
        {
            Task.Factory.StartNew(readCommands);
            await doListening();
        }

        private void readCommands()
        {
            while (!cancelationSource.Token.IsCancellationRequested)
            {
                string command = Console.ReadLine();
                if (command == "--bye") { cancelationSource.Cancel(); }
            }
        }

        private async Task doListening()
        {
            try
            {
                serverListener.Start();
                await listen(serverListener, cancelationSource.Token);
            }
            catch (SocketException ex)
            {
                log($"Network error (code: {ex.ErrorCode}): {ex.Message}");
            }
            finally
            {
                serverListener.Stop();
            }
        }

        private async Task listen(TcpListener listener, CancellationToken ct)
        {

            try
            {
                Task completedTask;
                while (!ct.IsCancellationRequested)
                {
                    var clientTask = listener.AcceptTcpClientAsync();
                    completedTask = await Task.WhenAny(clientTask, cancelationTask).ConfigureAwait(false);

                    if(completedTask == cancelationTask) { log("Server cancelation."); break; }

                    TcpClient client = await clientTask;
                    log("New client connected.");
                    var newClient = new ClientState(ct, _log);
                    newClient.Talk(client);

                }
            }
            catch (SocketException ex)
            {
                log($"Network error (code: {ex.ErrorCode}): {ex.Message}");
            }
        }

        private void log(string message)
        {
            if (_log) { Console.WriteLine(message); }
        }
    }  
}
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
