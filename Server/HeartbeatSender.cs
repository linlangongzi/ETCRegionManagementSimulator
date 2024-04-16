using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ETCRegionManagementSimulator
{
    public class HeartbeatSender : IDisposable
    {
        private readonly IPAddress mainServerIp;
        private readonly int mainServerPort;
        private readonly int heartbeatInterval;

        private TcpClient client;
        private NetworkStream stream;
        private Thread sendThread;
        private bool running = true;

        private bool disposedValue;

        public HeartbeatSender(IPAddress ip, int port, int heartbeatIntervalSeconds = 5)
        {
            mainServerIp = ip;
            mainServerPort = port;
            heartbeatInterval = heartbeatIntervalSeconds;
            client = new TcpClient();
        }

        public void StartSending()
        {
            client.Connect(mainServerIp, mainServerPort);
            stream = client.GetStream();
            sendThread = new Thread(new ThreadStart(SendHeartbeat));
            sendThread.Start();
        }

        private void SendHeartbeat()
        {
            try
            {
                while(running)
                {
                    byte[] buffer = Encoding.ASCII.GetBytes("Heartbeat");
                    stream.Write(buffer, 0, buffer.Length);
                    Thread.Sleep(heartbeatInterval * 1000);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error Sending hearbeat: " + e.Message);
            }
        }

        public void StopSending()
        {
            running = false;
            sendThread?.Join(); // Ensure the thread has completed
            stream.Close();
            client.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }
                disposedValue = true;
                StopSending();
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
