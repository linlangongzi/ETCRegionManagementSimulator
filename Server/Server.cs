using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ETCRegionManagementSimulator
{

    // Custom event arguments for client connected event
    public class ClientConnectedEventArgs : EventArgs
    {
        public string ClientId { get; }

        public ClientConnectedEventArgs(string clientId)
        {
            ClientId = clientId;
        }
        
    }

    public class Server : IDisposable
    {

        public event EventHandler<ClientConnectedEventArgs> ClientConnected;

        private IPAddress defaultIPAddress;
        private IPAddress backupIPAddress;
        private List<int> ports;

        private ConnectionsManager clientManager;
        // TODO: Need to Update listenerTaskList and Task management into Seperate Object
        //       Object name : Connection 
        private List<(TcpListener, Task)> listenerTaskList;
        public bool Running { get; set; }

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
            defaultIPAddress = IPAddress.Loopback;
            backupIPAddress = null;
            ports = new List<int>();
            listenerTaskList = new List<(TcpListener, Task)>();
            clientManager = new ConnectionsManager();
            Running = false;
        }

        public Server(string defaultIP, string backupIP, List<int> ports)
        {
            defaultIPAddress = IPAddress.Parse(defaultIP);
            backupIPAddress = IPAddress.Parse(backupIP);
            this.ports = ports;
            listenerTaskList = new List<(TcpListener, Task)>();
            clientManager = new ConnectionsManager();
            Running = false;
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
            ClientConnected?.Invoke(this, new ClientConnectedEventArgs(clientId));
        }

        private async Task AcceptClientsAsync(TcpListener listener, int port)
        {
            while (Running)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    System.Diagnostics.Debug.WriteLine($"Connection established on port {port}");

                    await Task.Run(() => HandleClientAsync(client));
                }
                catch(SocketException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error accepting client connection on port {port} : {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
        {
            // TODO: To change the Data retrieve method , should get this data from UI or Reading file
            string data = "this is test data from server  \n";
            string clientId = ClientIdGenerator.GenerateClientId();

            if (tcpClient != null)
            {
                Client client = new Client(clientId, tcpClient);
                clientManager.AddClient(clientId, client);

                // Raise the ClientConnected event;
                OnClientConnected(clientId);
                while (tcpClient.Connected)
                {
                    try
                    {
                        Task readDataTask = client.ReadDataAsync();
                        Task sendDataTask = client.SendDataAsync(data);
                
                        await Task.WhenAny(readDataTask, sendDataTask);

                        if (readDataTask.IsFaulted)
                        {
                            Exception readDataException = readDataTask.Exception;
                            System.Diagnostics.Debug.WriteLine($" Read Data Exception : {readDataException.Message}");
                            // TODO: Handle read data exception
                        }
                        else
                        {
                            Exception sendDataException = sendDataTask.Exception;
                            // TODO: Handle send data exception
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in communication with the client {clientId}: {ex.Message}");
                        break;
                    }
                }
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
                    if (clientManager != null)
                    {
                        clientManager.RemoveAllClients();
                        clientManager = null;
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
