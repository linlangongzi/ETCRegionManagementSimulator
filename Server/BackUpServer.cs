using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator
{
    public class BackUpServer : IDisposable
    {
        private bool disposedValue;

        private readonly IPAddress backupServerIp;
        private int backupServerPort;
        private readonly int heartbeatInterval;

        private HeartbeatReceiver heartbeatReceiver;
        private HeartbeatSender heartbeatSender;
        private CancellationTokenSource cancellationTokenSource;

        public BackUpServer(IPAddress mainServerIp, IPAddress backupIp, int port, int heartbeatIntervalSeconds = 5)
        {
            this.backupServerIp = backupIp;
            this.backupServerPort = port;
            this.heartbeatInterval = heartbeatIntervalSeconds;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.heartbeatReceiver = new HeartbeatReceiver(port, heartbeatInterval);
            this.heartbeatSender = new HeartbeatSender(mainServerIp, port, heartbeatInterval);
        }

        public void StartMainServerMonitoring()
        {
            heartbeatReceiver.StartListening();
            heartbeatSender.StartSending();
        }
        private void ActivateBackupOperations()
        {

            Debug.WriteLine("Main server is down. Backup server taking over.");
            // Implement the logic to start handling requests that were handled by the main server.
            // This might involve starting certain services, changing DNS entries, etc.
        }

        // Method to simulate main server recovery
        public void MainServerRecovered()
        {
            Debug.WriteLine("Main server has recovered. Switching back to main server operations.");
            // Implement the logic to switch back operations to the main server.
            // This might involve re-synchronizing data or other state details.
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancellationTokenSource?.Dispose();
                    heartbeatReceiver?.Dispose();
                    heartbeatSender?.Dispose();
                }

                heartbeatReceiver = null;
                heartbeatSender = null;
                cancellationTokenSource = null;
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BackUpServer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
