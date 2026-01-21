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
        public LuaEngine luaEngine = new LuaEngine();

        public void Start()
        {
            NetwerkrServer server = net.startServer();
            List<NetEntity> clientsData = new List<NetEntity>();

            server.Start();

            luaEngine.RegisterObject("server", new LuaServer(server));
            luaEngine.script.Globals["clientData"] = luaEngine.createFunction((Func<List<NetEntity>>)(() => clientsData));
            luaEngine.RunFile("scripts/server.lua");

            luaEngine?.Call("init");

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
                luaEngine?.Call("onNewClient", clientId);
                server.fireClient(clientId, "connect", clientsDataExept(clientId));
                clientsData.Add(new NetEntity(clientId));
            };

            server.listen("update", (clientId, data) =>
            {
                //Console.WriteLine($"Received update from {clientId}: {data}");
                var clientDataItem = clientsData.Find(c => c.clientId == clientId);
                if (clientDataItem != null)
                {
                    //luaEngine?.Call("updateRecieved", clientId, data);
                    clientDataItem.UpdateFromString(data);
                }
            });

            System.Timers.Timer broadcastTimer = new System.Timers.Timer(10);
            broadcastTimer.Start();

            broadcastTimer.Elapsed += (sender, e) =>
            {
                //luaEngine?.Call("preUpdateBroadcast");

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