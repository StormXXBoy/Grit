using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static void Main()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server running...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                clients.Add(client);

                Console.WriteLine("Client connected.");
                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }
        }

        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            void broadcast(string message)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);

                foreach (var c in clients.ToArray())
                {
                    if (c == client) continue;
                    try
                    {
                        NetworkStream s = c.GetStream();
                        s.Write(data, 0, data.Length);
                    }
                    catch
                    {
                        clients.Remove(c);
                    }
                }
            }

            broadcast("A new client has connected.");

            try
            {
                while (true)
                {
                    int byteCount = stream.Read(buffer, 0, buffer.Length);

                    if (byteCount == 0)
                        break;

                    string data = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine("Received: " + data);

                    broadcast("e");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client disconnected with error: " + ex.Message);
            }
            finally
            {
                clients.Remove(client);
                client.Close();
                Console.WriteLine("Client fully removed.");
            }
        }
    }
}
