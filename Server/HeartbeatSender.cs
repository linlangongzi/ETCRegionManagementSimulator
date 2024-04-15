using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ETCRegionManagementSimulator
{
    public class HeartbeatSender
    {
        private readonly string backupServerIp;
        private readonly int backupServerPort;
        private readonly int heartbeatInterval;

        public HeartbeatSender(string ip, int port, int heartbeatIntervalSeconds = 5)
        {
            this.backupServerIp = ip;
            this.backupServerPort = port;
            this.heartbeatInterval = heartbeatIntervalSeconds;
        }

        public void StartSending()
        {
            Thread sendThread = new Thread(new ThreadStart(SendHeartbeat));
            sendThread.Start();
        }

        private void SendHeartbeat()
        {
            using (UdpClient sender = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(backupServerIp), backupServerPort);
                while (true)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes("Heartbeat");
                    sender.Send(bytes, bytes.Length, endPoint);
                    Debug.WriteLine($"Heartbeat sent. by {this}");
                    Thread.Sleep(heartbeatInterval * 1000);
                }
            }
        }
    }
}
