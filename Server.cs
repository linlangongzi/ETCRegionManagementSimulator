using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator
{
    class Server
    {
        private IPAddress defaultIPAddress;
        private IPAddress backupIPAddress;
        private List<int> ports;
        private TcpListener listener;
        private bool running;

        public Server(string defaultIP, string backupIP, List<int> ports)
        {
            defaultIPAddress = IPAddress.Parse(defaultIP);
            backupIPAddress = IPAddress.Parse(backupIP);
            this.ports = ports;
            running = false;
        }

        public async Task Start()
        {
            if (!running)
            {
                running = true;
                listener = new TcpListener(defaultIPAddress, 0);  //Set port number to default 0 to initialize the listener  
            }
        }
    }
}
