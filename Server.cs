using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace ETCRegionManagementSimulator
{
    class Server
    {
        private IPAddress defaultIPAddress;
        private IPAddress backupIPAddress;
        private List<int> ports;

        private List<TcpListener> listeners;
        public bool Running { get; set; }

        private ClientsManager clientManager;

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
            listeners = new List<TcpListener>();
            clientManager = new ClientsManager();
            Running = false;
        }

        public Server(string defaultIP, string backupIP, List<int> ports)
        {
            defaultIPAddress = IPAddress.Parse(defaultIP);
            backupIPAddress = IPAddress.Parse(backupIP);
            this.ports = ports;
            listeners = new List<TcpListener>();
            clientManager = new ClientsManager();
            Running = false;
        }

        public async Task Start()
        {
            if (!Running)
            {
                Running = true;
                foreach(int port in ports)
                { 
                    TcpListener listener = new TcpListener(defaultIPAddress, port);  //Set port number to default 0 to initialize the listener  
                    listener.Start();
                    listeners.Add(listener);
                    Console.WriteLine($"Server started on port {port}");

                    // Start accepting clients Asynchronously
                    await AcceptClientsAsync(listener, port);
                }
            }
        }

        private async Task AcceptClientsAsync(TcpListener listener, int port)
        {
            while (Running)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    Console.WriteLine($"Connection established on port {port}");

                    await Task.Run(() => HandleClientAsync(client));
                }
                catch(SocketException ex)
                {
                    Console.WriteLine($"Error accepting client connection on port {port} : {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
        {
            string clientId = ClientIdGenerator.GenerateClientId();

            Client client = new Client(clientId, tcpClient);
            clientManager.AddClient(clientId, client);

            //Task readDataTask = client.ReadData();
            //Task sendDataTask = client.SendData();

            //await Task.WhenAny(readDataTask, sendDataTask);

            //if (readDataTask.IsFaulted)
            //{
            //    Exception readDataException = readDataTask.Exception;

            //    // TODO: Handle read data exception
            //}
            //else
            //{
            //    Exception sendDataException = sendDataTask.Exception;
            //    // TODO: Handle send data exception
            //}

        }


        public void Shutdown()
        {
            if (Running)
            {
                Running = false;
                // Destruct Clients in Client manager
                // 
                //listener.Stop();
                Console.WriteLine("Server stopped.");
            }
        }
    }
}
