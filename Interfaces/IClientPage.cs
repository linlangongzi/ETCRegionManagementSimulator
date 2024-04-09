using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator.Interfaces
{
    public interface IClientPage
    {
        string ClientId { get; }
        void UpdateViewFromMessage(string message);
    }
}
