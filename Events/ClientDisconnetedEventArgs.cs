using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Events
{
    public class ClientDisconnetedEventArgs : EventArgs
    {
        public string DisconnectClientId { get; }
        public ClientDisconnetedEventArgs(string clientId)
        {
            DisconnectClientId = clientId;
        }
    }
}
