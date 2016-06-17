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

        CancellationTokenSource cancelationTokenS;
        TcpListener serverListener;

        public MESServer()
        {
            cancelationTokenS = new CancellationTokenSource();
            serverListener = new TcpListener(IPAddress.Any, _port);
        }

        async public Task Work()
        {
            try
            {
                serverListener.Start();
                await listen(serverListener, cancelationTokenS.Token);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Network error (code: {ex.ErrorCode}): {ex.Message}");
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
                    var newClient = new ClientState(ct);

                    newClient.Talk(client);

                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Network error (code: {ex.ErrorCode}): {ex.Message}");
            }
        }
    }  
}
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
