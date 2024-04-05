using System;
using System.Collections.Generic;

namespace ETCRegionManagementSimulator
{
    public static class ClientIdGenerator
    {
        // TODO: Delete this utility class in future version
        // In future version the server may open connections as many as it can
        private const string clientId1 = "DATA_UPSTREAM_LINE";
        private const string clientId2 = "SERVER_HEALTH_CHECK";
        private const string clientId3 = "DATA_DOWNSTREAM_LINE";
        private const string clientId4 = "CLIENT_HEALTH_CHECK";
        private const string clientId5 = "CONTROL_LINE";

        private static Stack<string> IdPool = new Stack<string>();

        static ClientIdGenerator()
        {
            IdPool.Push(clientId1);
            IdPool.Push(clientId2);
            IdPool.Push(clientId3);
            IdPool.Push(clientId4);
            IdPool.Push(clientId5);
        }
        public static string GenerateClientId()
        {
            return IdPool.Pop();
        }

        public static string GenerateClientUuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}