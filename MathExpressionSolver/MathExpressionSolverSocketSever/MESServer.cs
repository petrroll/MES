using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

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
            serverListener.Start();
            await listen(serverListener, cancelationTokenS.Token);
            serverListener.Stop();
        }

        private async Task listen(TcpListener listener, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                var newClient = new ClientState(ct);
                newClient.Talk(client);
            }
        }
    }

   
}
