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
        private readonly int listenPort;
        private bool isMainServerAlive = true;
        private readonly int heartbeatInterval;
        private UdpClient listener;
        private Thread listenThread;
        private IPEndPoint groupEP;
        private CancellationTokenSource cancellationTokenSource;

        private bool disposedValue;
        public HeartbeatReceiver(int port, int heartbeatIntervalSeconds = 5)
        {
            this.listenPort = port;
            this.heartbeatInterval = heartbeatIntervalSeconds;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.groupEP = new IPEndPoint(IPAddress.Any, listenPort);
        }

        public void StartListening()
        {
            listener = new UdpClient(listenPort);
            listenThread = new Thread(new ThreadStart(ListenForHeartbeat));
            listenThread.Start();
        }

        private void ListenForHeartbeat()
        {
            try
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    Debug.WriteLine("Waiting for heartbeat...");
                    if (listener.Available > 0)
                    {
                        byte[] bytes = listener.Receive(ref groupEP);
                        if (bytes.Length > 0)
                        {
                            Debug.WriteLine("Heartbeat received.");
                            isMainServerAlive = true;
                        }
                    }

                    Thread.Sleep(heartbeatInterval * 1000);
                    if (!isMainServerAlive)
                    {
                        ActivateBackupOperations();
                        break;
                    }
                    // Reset flag for next interval
                    isMainServerAlive = false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($" Listen for heart beat {e} \n");
            }
            finally
            {
                listener.Close();
            }
        }

        private void ActivateBackupOperations()
        {
            Debug.WriteLine("Main Server is down.. Backup Server taking over \n");
            // Implement the logic to start handling requests that were handled by the 
        }

        public void StopListening()
        {
            cancellationTokenSource.Cancel();
            listener?.Close();
            listenThread?.Join(); // Ensure the thread has completed
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    listener?.Dispose();
                    cancellationTokenSource?.Dispose();
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
