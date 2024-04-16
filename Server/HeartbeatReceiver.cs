using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator
{
    public class HeartbeatReceiver : IDisposable
    {
        private TcpListener listener;
        private TcpClient client;
        private NetworkStream stream;
        private readonly int port;
        private bool running = true;
        private Thread listenThread;


        private bool disposedValue;
        public HeartbeatReceiver(int listenPort)
        {
            this.port = listenPort;
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void StartListening()
        {
            listener.Start();
            listenThread = new Thread(new ThreadStart(ListenForHeartbeats));
            listenThread.Start();
        }

        private void ListenForHeartbeats()
        {
            try
            {
                client = listener.AcceptTcpClient();
                stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (running)
                {
                    int byteRead = stream.Read(buffer, 0, buffer.Length);
                    if (byteRead > 0)
                    {
                        Debug.WriteLine($"Heartbeat received {buffer}");
                    }
                }
            } 
            catch (Exception ex)
            {
                Debug.WriteLine("Error receiving hearbeat: " + ex.Message);
            }
        }

        private void ActivateBackupOperations()
        {
            Debug.WriteLine("Main Server is down.. Backup Server taking over \n");
            // Implement the logic to start handling requests that were handled by the 
        }

        public void StopListening()
        {
            running = false;
            listenThread?.Join(); // Ensure the thread has completed
            stream.Close();
            client.Close();
            listener.Stop();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopListening();
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
