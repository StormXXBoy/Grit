using System.Net.Sockets;
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