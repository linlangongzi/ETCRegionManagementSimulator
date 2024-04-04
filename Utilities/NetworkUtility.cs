using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ETCRegionManagementSimulator
{
    public class Utility
    {
        public static List<int> GetAvailablePorts(int count = 5, int startPort = 5000)
        {
            List<int> availablePorts = new List<int>();
            int port = startPort;
            int foundCount = 0;

            while (foundCount < count)
            {
                if (IsPortAvailable(port))
                {
                    availablePorts.Add(port);
                    foundCount++;
                }
                port++;
            }
            return availablePorts;
        }

        private static bool IsPortAvailable(int port)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
