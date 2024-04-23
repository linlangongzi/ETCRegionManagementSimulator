using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

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
        public static string GetIPv4Address()
        {
            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                        foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving IPv4 address: " + ex.Message);
            }

            return null; // No IPv4 address found or an error occurred
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

        public static bool IsValidIpAddress(string ipAddress)
        {
            // Regular expression pattern for IPv4 address
            string pattern = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

            // Create Regex object
            Regex regex = new Regex(pattern);

            // Match the input string with the regex pattern
            Match match = regex.Match(ipAddress);

            // Return true if the input string matches the pattern, otherwise false
            return match.Success;
        }

    }
}
