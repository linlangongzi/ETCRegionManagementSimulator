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
    public class HeartbeatReceiver
    {
        private readonly int listenPort;
        private bool isMainServerAlive = true;
        private readonly int heartbeatInterval;

        public HeartbeatReceiver(int port, int heartbeatIntervalSeconds = 5)
        {
            this.listenPort = port;
            this.heartbeatInterval = heartbeatIntervalSeconds;
        }

        public void StartListening()
        {
            Thread listenThread = new Thread(new ThreadStart(ListenForHeartbeat));
            listenThread.Start();
        }

        private void ListenForHeartbeat()
        {
            using (UdpClient listerner = new UdpClient(listenPort))
            {
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
                try
                {
                    while (true)
                    {
                        Debug.WriteLine("Waiting for heartbeat....");
                        if (listerner.Receive(ref groupEP) != null)
                        {
                            Debug.WriteLine("HeartBeat received");
                            isMainServerAlive = true;
                        }

                        Thread.Sleep(heartbeatInterval * 1000);
                        if (!isMainServerAlive)
                        {
                            ActivateBackupOperations();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($" Listen for heart beat {e} \n");
                    // Handle exception appropriately
                }
            }
        }

        private void ActivateBackupOperations()
        {
            Debug.WriteLine("Main Server is down.. Backup Server taking over \n");
            // Implement the logic to start handling requests that were handled by the 
        }

    }
}
