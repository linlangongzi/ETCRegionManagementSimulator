using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ETCRegionManagementSimulator.Events;
using System.Diagnostics;
using System.Threading;

namespace ETCRegionManagementSimulator
{
    public class Server : IDisposable
    {

        public event EventHandler<ClientConnectedEventArgs> NewClientConnected;
        private TaskCompletionSource<bool> waitForSendSignal = new TaskCompletionSource<bool>();

        private IPAddress defaultIPAddress = IPAddress.Loopback;
        private IPAddress backupIPAddress = null;
        private List<int> ports = new List<int>();

        private ConnectionsManager connectionManager = new ConnectionsManager();
        // TODO: Need to Update listenerTaskList and Task management into Seperate Object
        //       Object name : Connection 
        private List<(TcpListener, Task)> listenerTaskList = new List<(TcpListener, Task)>();
        public bool Running { get; set; } = false;

        private bool disposedValue;

        public IPAddress DefaultIPAddress
        {
            get { return defaultIPAddress; }
            set { defaultIPAddress = value; }
        }

        public IPAddress BackupIPAddress
        {
            get { return backupIPAddress; }
            set { backupIPAddress = value; }
        }

        public List<int> Ports
        {
            get { return ports; }
            set { ports = value; }
        }

        // Default constructor
        public Server()
        {
            MainPage.SendSelectedData += OnMainPage_SendSelectedData;
        }

        public Server(string defaultIP, string backupIP, List<int> ports)
        {
            defaultIPAddress = IPAddress.Parse(defaultIP);
            backupIPAddress = IPAddress.Parse(backupIP);
            this.ports = ports;
            MainPage.SendSelectedData += OnMainPage_SendSelectedData;
        }

        private void OnMainPage_SendSelectedData(object sender, EventArgs e)
        {
            // Signal the TaskCompletionSource
            _ = waitForSendSignal.TrySetResult(true);
        }

        public Task Start()
        {
            if (!Running)
            {
                Running = true;
                foreach(int port in ports)
                {
                    TcpListener listener = new TcpListener(defaultIPAddress, port);
                    listener.Start();
                    System.Diagnostics.Debug.WriteLine($"Server started on port {port}");

                    // Start accepting clients Asynchronously
                    Task acceptTask = AcceptClientsAsync(listener, port);
                    listenerTaskList.Add((listener, acceptTask));
                }
            }

            return Task.CompletedTask;
        }

        // Method to raise the ClientConnected event
        protected virtual void OnClientConnected(string clientId)
        {
            NewClientConnected?.Invoke(this, new ClientConnectedEventArgs(clientId));
        }
        /// <summary>
        ///  TODO:
        ///  consider the following adjustments: 
        ///  Isolate TaskCompletionSource per Client:
        ///  It might be better to have a TaskCompletionSource instance per client connection rather than a shared one.This way, 
        ///  the signal to send data is specific to each client and can be controlled more granularly.
        ///  Reset TaskCompletionSource Safely: 
        ///  Ensure that the resetting of waitForSendSignal happens in a thread-safe manner to avoid potential race conditions.
        ///  This might involve locking or using thread-safe structures if multiple threads are accessing it.
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private async Task AcceptClientsAsync(TcpListener listener, int port)
        {
            string clientId = ClientIdGenerator.GenerateClientId();
            while (Running)
            {
                try
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();

                    Client client = new Client(clientId, tcpClient);
                    connectionManager.AddClient(clientId, client);
                    // Raise the ClientConnected event;
                    OnClientConnected(clientId);

                    Debug.WriteLine($"Connection established on port {port}");

                    await Task.Run(() => HandleClientAsync(client));
                }
                catch(SocketException ex)
                {
                    Debug.WriteLine($"Error accepting client connection on port {port} : {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(Client client)
        {
            // TODO: To change the Data retrieve method , should get this data from UI or Reading file
            string data = "this is test data from server \n";

            if (client != null)
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                try
                {
                    Task readDataTask = Task.Run(() => ReadDataContinuously(client, cancellationTokenSource.Token));

                    while (client.TcpClient.Connected)
                    {
                        await waitForSendSignal.Task;
                        await client.SendDataAsync(data);
                        waitForSendSignal = new TaskCompletionSource<bool>();
                    }
                    await readDataTask;

                }
                catch (Exception ex) when (ex is SocketException || ex is IOException)
                {
                    cancellationTokenSource.Cancel();
                    Debug.WriteLine($"Network error in communication with the client {client.Id}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    cancellationTokenSource.Cancel();
                    Debug.WriteLine($"Unhandled error with the client {client.Id} : {ex.Message}");
                }
                finally
                {
                    // Cancel the reading task if it's still running
                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                    }
                    cancellationTokenSource.Cancel();
                    // sending a specific message to the client indicating disconnection
                    if (client.TcpClient.Connected)
                    {
                        /// TODO; signal the client that the server is closing the connection in the future release
                        client.TcpClient.Close();
                    }
                    //// Ensure the connection is closed and resources are freed
                    //client.TcpClient.Close();
                    connectionManager.RemoveClient(client.Id);
                    Debug.WriteLine($"Connection with client {client.Id} closed.");
                    cancellationTokenSource.Dispose();
                }
            }
        }

        private async Task ReadDataContinuously(Client client, CancellationToken cancellationToken)
        {
            // Keep reading data from the client independently of the send signal.
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    string receivedData = await client.ReadDataAsync(); // Assuming this method exists and returns a Task<string>
                    if (!string.IsNullOrEmpty(receivedData))
                    {
                        // Process the received data. For example, log it, or trigger events based on content.
                        Debug.WriteLine($"Received data from client {client.Id}: {receivedData}");
                    }
                    else if (cancellationToken.IsCancellationRequested)
                    {
                        // Handle the cancellation due to client disconnection
                        Debug.WriteLine("Cancellation requested due to client disconnection.");
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Read operation was canceled.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions, such as network errors.
                Debug.WriteLine($"Error while reading data from client {client.Id}: {ex.Message}");
            }
            finally
            {
                // Perform any cleanup, remove the client from the connection manager.
                if (client.TcpClient.Connected)
                {
                    client.TcpClient.Close();
                }
                connectionManager.RemoveClient(client.Id);
                Debug.WriteLine($"Stopped reading data from client {client.Id}.");
            }
        }

        private void StopTasks()
        {
            if (Running)
            {
                Running = false;
                // Stop all listeners and wait for associated tasks to complete
                foreach (var (listener, task) in listenerTaskList)
                {
                    listener.Stop();
                }
                // Wait for all associated tasks to complete
                Task.WhenAll(listenerTaskList.Select(pair => pair.Item2));
                // Clear the listenerTasks list
                listenerTaskList.Clear();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (connectionManager != null)
                    {
                        connectionManager.RemoveAllClients();
                        connectionManager = null;
                    }

                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                StopTasks();
                Running = false;
                disposedValue = true;
            }
        }

        ~Server()
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
