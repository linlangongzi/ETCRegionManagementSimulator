using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator
{
    class ConnectionsManager
    {
        private Dictionary<string, Client> clients = new Dictionary<string, Client>();

        public void AddClient(string clientId, Client client) 
        {
            lock (clients) 
            {
                clients.Add(clientId, client);
            }
        }

        public void RemoveClient(string clientId)
        {
            lock (clients)
            { 
                clients.Remove(clientId);
            }
        }

        public void RemoveAllClients()
        {
            lock (clients)
            {
                foreach(var client in clients)
                {
                    client.Value.Dispose();
                }
                clients.Clear();
            }
        }

        public Dictionary<string, Client> GetAllClients()
        {
            return clients;
        }
        public Client GetClient(string clientId)
        {
            lock (clients)
            {
                if (clients.ContainsKey(clientId))
                {
                    return clients[clientId];
                }
                else
                {
                    return null;
                }
            }
        }

        public List<string> GetClientIds()
        {
            lock(clients)
            {
                return new List<string>(clients.Keys);
            }
        }

        public async Task BroadcastDataToSelectedClients(List<Client> clients, string data)
        {
            List<Task> tasks = (from client in clients
                                let task = client.SendDataAsync(data)
                                select task).ToList();
            await Task.WhenAll(tasks);
        }

        public async Task BroadcastDataToALL(string data)
        {
            await BroadcastDataToSelectedClients(new List<Client>(clients.Values), data);
        }
    }
}
