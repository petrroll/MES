﻿namespace MESExamples.SocketServer
{
    class Program
    {
#pragma warning disable RECS0154 // Parameter is never used
        static void Main(string[] args)
#pragma warning restore RECS0154 // Parameter is never used
        {
            var server = new MESServer(true);
            server.Work().Wait();
        }
    }
}
