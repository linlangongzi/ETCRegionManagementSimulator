using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Events
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }

        public string ClientId { get; }

        public MessageReceivedEventArgs(string senderId, string message)
        {
            ClientId = senderId;
            Message = message;
        }
    }
}
