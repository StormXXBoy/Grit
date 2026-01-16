using GritNetworking;
using Netwerkr;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Server
{
    public class Program
    {
        private Netwerkr.Netwerkr net = new Netwerkr.Netwerkr();

        public void Start()
        {
            NetwerkrServer server = net.startServer();
            server.Start();

            List<NetEntity> clientsData = new List<NetEntity>();

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
                clientsData.Add(new NetEntity(clientId));
            };

            server.listen("update", (clientId, data) =>
            {
                Console.WriteLine($"Received update from {clientId}: {data}");
                var clientDataItem = clientsData.Find(c => c.clientId == clientId);
                if (clientDataItem != null)
                {
                    clientDataItem.UpdateFromString(data);
                }
            });

            System.Timers.Timer broadcastTimer = new System.Timers.Timer(10);
            broadcastTimer.Start();

            broadcastTimer.Elapsed += (sender, e) =>
            {
                foreach (var item in clientsData)
                {
                    server.fireClient(item.clientId, "update", clientsDataExept(item.clientId));
                }
            };
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Start();
            while (true)
            {
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}