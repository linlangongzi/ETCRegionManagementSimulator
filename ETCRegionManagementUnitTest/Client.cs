using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace ETCRegionManagementUnitTest
{
    class Client
    {
        private StreamSocket socket;
        private DataReader reader;
        private DataWriter writer;
        private bool isConnected = false;

        public bool IsConnected
        {
            get { return isConnected; }
        }

        public async Task<string> ConnectAsync(string serverIpAddress, int serverPort)
        {
            try
            {
                socket = new StreamSocket();
                await socket.ConnectAsync(new HostName(serverIpAddress), serverPort.ToString());
                reader = new DataReader(socket.InputStream);
                writer = new DataWriter(socket.OutputStream);
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
                throw new InvalidOperationException("Client is not connected.");
            }

            writer.WriteBytes(data);
            await writer.StoreAsync();
            await writer.FlushAsync();
        }

        public async Task<byte[]> ReceiveDataAsync()
        {
            if (!isConnected)
            {
                throw new InvalidOperationException("Client is not connected.");
            }

            try
            {
                uint bytesRead = await reader.LoadAsync(sizeof(uint));
                if (bytesRead < sizeof(uint))
                {
                    throw new IOException("Could not read data size.");
                }

                uint dataSize = reader.ReadUInt32();
                bytesRead = await reader.LoadAsync(dataSize);
                if (bytesRead < dataSize)
                {
                    throw new IOException("Could not read full data.");
                }

                byte[] receivedData = new byte[dataSize];
                reader.ReadBytes(receivedData);
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
                return $"Connected to {socket.Information.RemoteAddress.DisplayName} : {socket.Information.RemotePort}.";
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
                if (socket != null)
                {
                    socket.Dispose();
                    socket = null;
                }
                if (reader != null)
                {
                    reader.DetachStream();
                    reader = null;
                }
                if (writer != null)
                {
                    writer.DetachStream();
                    writer = null;
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

