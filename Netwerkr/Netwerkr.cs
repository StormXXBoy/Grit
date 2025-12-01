<<<<<<< HEAD
﻿using System.Net.Sockets;
using System.Threading.Tasks;

namespace Netwerkr
{
    public static class Netwerkr
    {
        public static async Task<NetwerkrConnection> connect(string ip, int port = 3000)
        {
            var client = new TcpClient();
            await client.ConnectAsync(ip, port);
            return new NetwerkrConnection(client);
        }
    }

}
=======
﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Netwerkr
{
    public class Netwerkr
    {
        public const int defaultPort = 3000;

        public NetwerkrServer startServer(int port = defaultPort)
        {
            return new NetwerkrServer(port);
        }

        public NetwerkrClient startClient(string ip, int port = defaultPort)
        {
            return new NetwerkrClient(ip, port);
        }
    }

    public class NetwerkrPacket
    {
        public string Event;
        public string Data;

        public NetwerkrPacket() { }

        public NetwerkrPacket(string eventName, string data)
        {
            Event = eventName;
            Data = data;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static NetwerkrPacket Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<NetwerkrPacket>(json);
        }

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(Serialize());
        }

        public static NetwerkrPacket FromBytes(byte[] bytes, int count)
        {
            string json = Encoding.UTF8.GetString(bytes, 0, count);
            return Deserialize(json);
        }
    }

    public class NetwerkrServer
    {
        public TcpListener listener;
        public Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        public Action<string> clientConnected = null;
        private Dictionary<string, Action<string, string>> eventHandlers = new Dictionary<string, Action<string, string>>();

        public NetwerkrServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(OnClientAccepted, null);
        }

        public void listen(string eventName, Action<string, string> callback)
        {
            eventHandlers[eventName] = callback;
        }

        private void OnClientAccepted(IAsyncResult result)
        {
            TcpClient tcp = listener.EndAcceptTcpClient(result);
            listener.BeginAcceptTcpClient(OnClientAccepted, null);

            string clientId = Guid.NewGuid().ToString();
            clients[clientId] = tcp;

            NetworkStream stream = tcp.GetStream();
            StateObject state = new StateObject();
            state.Client = tcp;
            state.Stream = stream;
            state.ClientId = clientId;

            clientConnected?.Invoke(clientId);

            stream.BeginRead(state.Buffer, 0, state.Buffer.Length, OnDataReceived, state);
        }

        private void OnDataReceived(IAsyncResult result)
        {
            StateObject state = (StateObject)result.AsyncState;

            int bytes;
            try
            {
                bytes = state.Stream.EndRead(result);
                if (bytes <= 0)
                {
                    clients.Remove(state.ClientId);
                    return;
                }
            }
            catch
            {
                clients.Remove(state.ClientId);
                return;
            }

            try
            {
                NetwerkrPacket packet = NetwerkrPacket.FromBytes(state.Buffer, bytes);
                if (packet != null && eventHandlers.ContainsKey(packet.Event))
                    eventHandlers[packet.Event]?.Invoke(state.ClientId, packet.Data);
            }
            catch { }

            state.Stream.BeginRead(state.Buffer, 0, state.Buffer.Length, OnDataReceived, state);
        }

        public void fireClient(string clientId, string eventName, string data)
        {
            if (!clients.ContainsKey(clientId)) return;

            TcpClient tcp = clients[clientId];
            NetworkStream stream = tcp.GetStream();

            NetwerkrPacket packet = new NetwerkrPacket(eventName, data);
            byte[] bytes = packet.ToBytes();
            stream.Write(bytes, 0, bytes.Length);
        }

        public void fireAllClients(string eventName, string data)
        {
            foreach (var id in new List<string>(clients.Keys))
                fireClient(id, eventName, data);
        }
    }

    public class NetwerkrClient
    {
        public TcpClient client;
        public NetworkStream stream;

        private Dictionary<string, Action<string>> eventHandlers = new Dictionary<string, Action<string>>();

        public NetwerkrClient(string ip, int port)
        {
            client = new TcpClient();
            client.Connect(ip, port);
            stream = client.GetStream();

            StateObject state = new StateObject();
            state.Client = client;
            state.Stream = stream;

            stream.BeginRead(state.Buffer, 0, state.Buffer.Length, OnDataReceived, state);
        }

        public void listen(string eventName, Action<string> callback)
        {
            eventHandlers[eventName] = callback;
        }

        private void OnDataReceived(IAsyncResult result)
        {
            StateObject state = (StateObject)result.AsyncState;

            int bytes;
            try
            {
                bytes = state.Stream.EndRead(result);
                if (bytes <= 0)
                    return;
            }
            catch
            {
                return;
            }

            try
            {
                NetwerkrPacket packet = NetwerkrPacket.FromBytes(state.Buffer, bytes);
                if (packet != null && eventHandlers.ContainsKey(packet.Event))
                    eventHandlers[packet.Event]?.Invoke(packet.Data);
            }
            catch { }

            state.Stream.BeginRead(state.Buffer, 0, state.Buffer.Length, OnDataReceived, state);
        }

        public void fire(string eventName, string data)
        {
            NetwerkrPacket packet = new NetwerkrPacket(eventName, data);
            byte[] bytes = packet.ToBytes();
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    public class StateObject
    {
        public TcpClient Client;
        public NetworkStream Stream;
        public string ClientId;
        public byte[] Buffer = new byte[4096];
    }
}
>>>>>>> master
