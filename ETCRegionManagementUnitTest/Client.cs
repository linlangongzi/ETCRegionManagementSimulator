using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementUnitTest
{
    class Client
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private bool isConnected = false;

        public bool IsConnected => isConnected;

        public async Task<string> ConnectAsync(string serverIpAddress, int serverPort)
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIpAddress, serverPort);
                networkStream = tcpClient.GetStream();
                isConnected = true;
                System.Diagnostics.Debug.WriteLine($"Connected to {serverIpAddress} : {serverPort}.");
                return $"Connected to {serverIpAddress} : {serverPort}.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Connection failed: {ex.Message}");
                return $"Connection failed: {ex.Message}";
            }
        }

        public async Task SendDataAsync(byte[] data)
        {
            if (!isConnected)
            {
                System.Diagnostics.Debug.WriteLine("Client is not connected.");
                return;
            }
            System.Diagnostics.Debug.WriteLine($"Attempt to send {Encoding.UTF8.GetString(data)}");
            await networkStream.WriteAsync(data, 0, data.Length);
            System.Diagnostics.Debug.WriteLine($"sent");

        }

        public async Task<byte[]> ReceiveDataAsync()
        {
            if (!isConnected)
            {
                System.Diagnostics.Debug.WriteLine("Client is not connected.");
            }

            try
            {
                byte[] dataSizeBuffer = new byte[sizeof(int)];
                await networkStream.ReadAsync(dataSizeBuffer, 0, dataSizeBuffer.Length);
                int dataSize = BitConverter.ToInt32(dataSizeBuffer, 0);

                byte[] receivedData = new byte[dataSize];
                int totalBytesRead = 0;
                while (totalBytesRead < dataSize)
                {
                    int bytesRead = await networkStream.ReadAsync(receivedData, totalBytesRead, dataSize - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Connection closed while reading data.");
                    }
                    totalBytesRead += bytesRead;
                }

                return receivedData;
            }
            catch (Exception ex)
            {
                // Handle error while receiving data
                // ex.Message will contain the error message
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<string> GetConnectionStatus()
        {
            if (isConnected)
            {
                return $"Connected to {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address} : {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port}, from{((IPEndPoint)tcpClient.Client.LocalEndPoint).Port}.";
            }
            else
            {
                return "Disconnected.";
            }
        }

        public async Task<string> DisconnectAsync()
        {
            try
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    tcpClient = null;
                }
                if (networkStream != null)
                {
                    networkStream.Close();
                    networkStream = null;
                }
                isConnected = false;
                return "Disconnected.";
            }
            catch (Exception ex)
            {
                return $"Disconnection failed: {ex.Message}";
            }
        }
    }
}
