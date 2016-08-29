using System;
using System.IO;
using System.Net.Sockets;

namespace MESExamples.SocketClient
{
    class Client
    {
        TcpClient client;
        private const int _port = 4586;

        public void Talk(string server = "127.0.0.1")
        {
            try
            {
                using (client = new TcpClient(server, _port))
                {
                    string command = Console.ReadLine();

                    using (client)
                    using (var clientStream = client.GetStream())
                    using (var reader = new StreamReader(clientStream))
                    using (var writer = new StreamWriter(clientStream) { AutoFlush = true })
                    {
                        while (true)
                        {
                            writer.WriteLine(command);
                            if (command == "--bye") { break; }

                            string response = reader.ReadLine();
                            Console.WriteLine(response);

                            command = Console.ReadLine();
                        }


                    }
                }
            }
            catch(SocketException ex)
            {
                Console.WriteLine($"Network error (code: {ex.ErrorCode}): {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Network error: {ex.Message}");
            }

        }

    }
}
