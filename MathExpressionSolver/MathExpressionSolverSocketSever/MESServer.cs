using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
namespace MathExpressionSolverSocketSever
{
    class MESServer
    {
        private const int _port = 4586;
        bool _log;

        CancellationTokenSource cancelationTokenS;
        TcpListener serverListener;

        public MESServer(bool log)
        {
            _log = log;
            cancelationTokenS = new CancellationTokenSource();
            serverListener = new TcpListener(IPAddress.Any, _port);
        }

        async public Task Work()
        {
            await doListening();
        }

        private async Task doListening()
        {
            try
            {
                serverListener.Start();
                await listen(serverListener, cancelationTokenS.Token);
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
                while (!ct.IsCancellationRequested)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
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
