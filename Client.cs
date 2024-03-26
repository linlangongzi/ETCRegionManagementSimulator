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

        public string ReadData()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = Stream.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public void SendData(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            Stream.Write(buffer, 0, buffer.Length);
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
