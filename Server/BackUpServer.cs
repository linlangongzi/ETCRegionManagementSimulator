using System;
using System.Collections.Generic;
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
        private readonly IPAddress mainServerIp = IPAddress.Loopback;
        private readonly int checkInterval;
        private bool mainServerActiveState = true;

        private bool disposedValue;

        public BackUpServer(IPAddress mainServerIp, int checkIntervalSeconds = 5)
        {
            this.mainServerIp = mainServerIp;
            checkInterval = checkIntervalSeconds;
        }

        // Start monitoring the main servver
        public void StartMonitoring()
        {
            Thread monitorThread = new Thread(new ThreadStart(MonitorMainServer));
            monitorThread.Start();
        }

        private void MonitorMainServer()
        {
            while (true)
            {
                mainServerActiveState = CheckServerStatus(mainServerIp);
                if (!mainServerActiveState)
                {
                    ActivateBackupOperations();
                    break;
                }
                Thread.Sleep(checkInterval * 1000);
            }
        }

        private bool CheckServerStatus(IPAddress ip)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(ip, 1000);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        private void ActivateBackupOperations()
        {

            Console.WriteLine("Main server is down. Backup server taking over.");
            // Implement the logic to start handling requests that were handled by the main server.
            // This might involve starting certain services, changing DNS entries, etc.
        }

        // Method to simulate main server recovery
        public void MainServerRecovered()
        {
            mainServerActiveState = true;
            Console.WriteLine("Main server has recovered. Switching back to main server operations.");
            // Implement the logic to switch back operations to the main server.
            // This might involve re-synchronizing data or other state details.
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
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
