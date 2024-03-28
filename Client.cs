using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ETCRegionManagementSimulator
{
    class Client
    {
        public string Id { get; }
        public TcpClient TcpClient { get; }
        public NetworkStream Stream { get; }

        public Client(string id, TcpClient client)
        {
            Id = id;
            TcpClient = client;
            Stream = TcpClient.GetStream();
        }

        public async Task<string> ReadDataAsync()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await Stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public async Task SendDataAsync(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            await Stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public void AnalyzeData(string data)
        {
            // Your data analysis logic goes here
            Console.WriteLine($"Analyzing data from client {Id}: {data}");
        }

        public void ToLog(string data)
        {
            Console.WriteLine($"Analyzing data from client {Id}: {data}");
        }
    }
}
