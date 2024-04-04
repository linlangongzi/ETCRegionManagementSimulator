﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ETCRegionManagementSimulator
{
    class Client : IDisposable
    {
        private bool disposedValue;
        public string Id { get; }
        public TcpClient TcpClient { get; }
        public NetworkStream Stream { get; }

        public Client(string id, TcpClient client)
        {
            Id = id;
            TcpClient = client;
            Stream = TcpClient.GetStream();
        }

        public async Task<string> ReadDataAsync()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await Stream.ReadAsync(buffer, 0, buffer.Length);
            System.Diagnostics.Debug.Write($"Received data from client({Id}): {Encoding.UTF8.GetString(buffer, 0, bytesRead)}");
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public async Task SendDataAsync(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            await Stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public void AnalyzeData(string data)
        {
            // Your data analysis logic goes here
            System.Diagnostics.Debug.WriteLine($"Analyzing data from client {Id}: {data}");
        }

        public void ToLog(string data)
        {
            System.Diagnostics.Debug.WriteLine($"Analyzing data from client {Id}: {data}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Stream?.Dispose();
                    TcpClient?.Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~Client()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
