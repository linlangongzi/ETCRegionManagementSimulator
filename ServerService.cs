using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCRegionManagementSimulator
{
    public class ServerService
    {
        private static ServerService _instance;

        private ServerService() { }

        public static ServerService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServerService();
                }
                return _instance;
            }
        }
        public Server Server { get; } = new Server();
    }
}
