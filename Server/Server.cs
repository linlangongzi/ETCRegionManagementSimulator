using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ETCRegionManagementSimulator.Events;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;

namespace ETCRegionManagementSimulator
{
    public class Server : IDisposable
    {

        public event EventHandler<ClientConnectedEventArgs> NewClientConnected;
        public event EventHandler<ClientDisconnetedEventArgs> ClientDisconnected;

        private TaskCompletionSource<bool> waitForSendSignal = new TaskCompletionSource<bool>();

        private IPAddress defaultIPAddress = IPAddress.Loopback;
        private IPAddress backupIPAddress = null;
        private List<int> ports = new List<int>();

        private ConnectionsManager connectionManager = new ConnectionsManager();
        // TODO: Need to Update listenerTaskList and Task management into Seperate Object
        //       Object name : Connection 
        private List<(TcpListener, Task)> listenerTaskList = new List<(TcpListener, Task)>();

        private ConcurrentQueue<string> dataQueue = new ConcurrentQueue<string>();

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
            if (e is SendSelectedDataEventArgs args)
            {
                string data = args.SelectedData;
                dataQueue.Enqueue(data);
                _ = waitForSendSignal.TrySetResult(true);
            }
        }

        public Task Start()
        {
            if (!Running)
            {
                Running = true;
                foreach (int port in ports)
                {
                    TcpListener listener = new TcpListener(defaultIPAddress, port);
                    listener.Start();
                    Debug.WriteLine($"Server started on port {port}");

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
        ///  TODO: Await Pattern and Loop Logic 
        ///  Your current implementation waits for either the read or send task to complete, 
        ///  then checks if one has completed but the other hasn't,
        ///  and awaits the incomplete task. This logic could be simplified, 
        ///  and the usage of waitForSendSignal might need to be adjusted to align with your requirements.
        ///  consider the following adjustments: 
        ///  Isolate TaskCompletionSource per Client:
        ///  It might be better to have a TaskCompletionSource instance per client connection rather than a shared one.This way, 
        ///  the signal to send data is specific to each client and can be controlled more granularly.
        ///  Reset TaskCompletionSource Safely: 
        ///  Ensure that the resetting of waitForSendSignal happens in a thread-safe manner to avoid potential race conditions.
        ///  This might involve locking or using thread-safe structures if multiple threads are accessing it.
        /// </summary>

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
            if (client != null)
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                try
                {
                    Task readDataTask = Task.Run(() => ReadDataContinuously(client, cancellationTokenSource.Token));

                    while (client.TcpClient.Connected)
                    {
                        await waitForSendSignal.Task;

                        if (dataQueue.TryDequeue(out string dataToSend))
                        {
                            await client.SendDataAsync(dataToSend);
                        }
                        else
                        {
                            Debug.WriteLine("No data to send \n");
                        }
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
                        cancellationTokenSource.Cancel();
                    }
                    // sending a specific message to the client indicating disconnection
                    if (client.TcpClient.Connected)
                    {
                        /// TODO; signal the client that the server is closing the connection in the future release
                        client.TcpClient.Close();
                    }
                    //// Ensure the connection is closed and resources are freed
                    //client.TcpClient.Close();
                    connectionManager.RemoveClient(client.Id);
                    client.Dispose();
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
                        Debug.WriteLine($"No data received from client {client.Id}, possible disconnection.");
                        ClientDisconnected?.Invoke(this, new ClientDisconnetedEventArgs(client.Id));
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
                client.Dispose();
                Debug.WriteLine($"Stopped reading data from client {client.Id}.");
            }
        }

        public void StopTasks()
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
