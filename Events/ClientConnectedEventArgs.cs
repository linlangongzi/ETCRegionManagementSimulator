using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Events
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public string ClientId { get; }
        public ClientConnectedEventArgs(string clientId)
        {
            ClientId = clientId;
        }

    }
}
