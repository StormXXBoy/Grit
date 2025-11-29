using Netwerkr;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Server
{
    class clientData
    {
        public string clientId;
        public string posX = "0";
        public string posY = "0";
        public string velX = "0";
        public string velY = "0";

        public clientData(string id)
        {
            clientId = id;
        }

        public override string ToString()
        {
            return $"{clientId}/{posX}/{posY}/{velX}/{velY}";
        }
    }

    class Program
    {
        private Netwerkr.Netwerkr net = new Netwerkr.Netwerkr();

        void Start()
        {
            NetwerkrServer server = net.startServer();
            server.Start();

            List<clientData> clientsData = new List<clientData>();

            string clientsDataExept(string clientId)
            {
                string dataToSend = "";
                foreach (var item in clientsData)
                {
                    if (item.clientId == clientId) continue;
                    dataToSend += item.ToString() + "|";
                }
                dataToSend = dataToSend.TrimEnd('|');
                return dataToSend;
            }

            server.clientConnected = (clientId) =>
            {
                Console.WriteLine($"Client connected: {clientId}");
                server.fireClient(clientId, "connect", clientsDataExept(clientId));
                clientsData.Add(new clientData(clientId));
            };

            server.listen("update", (clientId, data) =>
            {
                Console.WriteLine($"Received update from {clientId}: {data}");
                var parts = data.Split('/');
                var clientDataItem = clientsData.Find(c => c.clientId == clientId);
                if (clientDataItem != null)
                {
                    clientDataItem.posX = parts[0];
                    clientDataItem.posY = parts[1];
                    clientDataItem.velX = parts[2];
                    clientDataItem.velY = parts[3];
                }
            });

            while (true)
            {
                System.Threading.Thread.Sleep(10);
                foreach (var item in clientsData)
                {
                    server.fireClient(item.clientId, "update", clientsDataExept(item.clientId));
                }
            }
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Start();
        }
    }
}