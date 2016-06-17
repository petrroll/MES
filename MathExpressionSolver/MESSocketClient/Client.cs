﻿using System;
using System.IO;
using System.Net.Sockets;

namespace MESSocketClient
{
    class Client
    {
        TcpClient client;
        private const int _port = 4586;

        public void Talk(string server = "127.0.0.1")
        {
            using (client = new TcpClient(server, _port))
            {
                string command = Console.ReadLine();

                using (client)
                using (var clientStream = client.GetStream())
                using (var reader = new StreamReader(clientStream))
                using (var writer = new StreamWriter(clientStream))
                {
                    while (true)
                    {
                        writer.WriteLine(command);
                        writer.Flush();
                        if (command == "--bye") { break; }

                        string response = reader.ReadLine();
                        Console.WriteLine(response);

                        command = Console.ReadLine();
                    }


                }
            }
        }

    }
}
