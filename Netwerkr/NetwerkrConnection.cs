using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Netwerkr
{
    public class NetwerkrConnection
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly Dictionary<string, Eventr> _events = new();

        public NetwerkrConnection(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
            StartReading();
        }

        public Eventr Listen(string eventName)
        {
            if (!_events.ContainsKey(eventName))
                _events[eventName] = new Eventr(eventName);

            return _events[eventName];
        }

        public async void Fire(string eventName, string data)
        {
            string packet = eventName + ":" + data + "\n";
            byte[] bytes = Encoding.UTF8.GetBytes(packet);
            await _stream.WriteAsync(bytes, 0, bytes.Length);
        }

        private async void StartReading()
        {
            var buffer = new byte[4096];
            var sb = new StringBuilder();

            while (true)
            {
                int bytes = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytes <= 0) continue;

                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytes));

                while (sb.ToString().Contains("\n"))
                {
                    string line = sb.ToString();
                    int index = line.IndexOf('\n');
                    string fullPacket = line.Substring(0, index);
                    sb.Remove(0, index + 1);

                    HandlePacket(fullPacket);
                }
            }
        }

        private void HandlePacket(string packet)
        {
            int sep = packet.IndexOf(':');
            if (sep < 0) return;

            string eventName = packet.Substring(0, sep);
            string payload = packet.Substring(sep + 1);

            if (_events.TryGetValue(eventName, out var evt))
            {
                evt.Invoke(payload);
            }
        }
    }

}
