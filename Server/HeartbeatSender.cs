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
        private readonly IPAddress backupServerIp;
        private readonly int backupServerPort;
        private readonly int heartbeatInterval;

        private Thread sendThread;
        private UdpClient sender;
        private IPEndPoint endPoint;
        private CancellationTokenSource cancellationTokenSource;

        private bool disposedValue;

        public HeartbeatSender(IPAddress ip, int port, int heartbeatIntervalSeconds = 5)
        {
            this.backupServerIp = ip;
            this.backupServerPort = port;
            this.heartbeatInterval = heartbeatIntervalSeconds;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.sender = new UdpClient();
            this.endPoint = new IPEndPoint(backupServerIp, backupServerPort);
        }

        public void StartSending()
        {
            sendThread = new Thread(new ThreadStart(SendHeartbeat));
            sendThread.Start();
        }

        private void SendHeartbeat()
        {
            try
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes("Heartbeat");
                    sender.Send(bytes, bytes.Length, endPoint);  // Use the persistent UdpClient instance
                    Debug.WriteLine($"Heartbeat sent by {this}");
                    Thread.Sleep(heartbeatInterval * 1000);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to send heartbeat: {e.Message}");
                // Optionally rethrow or handle the exception
            }
            finally
            {
                sender.Close();
            }
        }

        public void StopSending()
        {
            cancellationTokenSource.Cancel();
            sendThread?.Join(); // Ensure the thread has completed
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancellationTokenSource?.Dispose();
                    sender?.Dispose();
                }
                disposedValue = true;
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
