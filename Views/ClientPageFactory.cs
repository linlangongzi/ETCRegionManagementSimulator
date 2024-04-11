using ETCRegionManagementSimulator.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ETCRegionManagementSimulator.Views
{
    public class ClientPageFactory
    {
        private ConcurrentDictionary<string, IClientPage> pagesById = new ConcurrentDictionary<string, IClientPage>();

        private static readonly Lazy<ClientPageFactory> instance = new Lazy<ClientPageFactory>(() => new ClientPageFactory());
        public static ClientPageFactory Instance => instance.Value;
        private ClientPageFactory() { }

        public IClientPage CreateClientPage(string clientId)
        {
            if (!pagesById.ContainsKey(clientId))
            {
                StandardPage clientPage = new StandardPage();
                clientPage.ClientPageId = clientId;
                pagesById[clientId] = clientPage;
                return clientPage;
            }
            else
            {
                return pagesById[clientId];
            }
        }

        public IClientPage GetPageById(string clientId)
        {
            _ = pagesById.TryGetValue(clientId, out IClientPage page);
            return page;
        }

        public IClientPage RemoveClientPage(string clientId)
        {
            _ = pagesById.TryRemove(clientId, out IClientPage page);
            return page;
        }
    }
}
