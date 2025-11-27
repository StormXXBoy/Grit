using Netwerkr;
using System;

namespace Server
{
    class Program
    {
        private Netwerkr.Netwerkr net = new Netwerkr.Netwerkr();
        private NetwerkrServer server;

        void Start()
        {
            server = net.startServer();
            server.Start();

            server.listen("test", (clientId, data) =>
            {
                Console.WriteLine($"Received from {clientId}: {data}");

            });

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Start();
        }
    }
}