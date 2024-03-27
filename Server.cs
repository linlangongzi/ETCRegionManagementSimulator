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
        private bool running;
        private ClientsManager clientManager;

        public Server(string defaultIP, string backupIP, List<int> ports)
        {
            defaultIPAddress = IPAddress.Parse(defaultIP);
            backupIPAddress = IPAddress.Parse(backupIP);
            this.ports = ports;
            clientManager = new ClientsManager();
            running = false;
        }

        public async Task Start()
        {
            if (!running)
            {
                running = true;
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
            while (running)
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

            client.ReadData();
        }
    }
}
