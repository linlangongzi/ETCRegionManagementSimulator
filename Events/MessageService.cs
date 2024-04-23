using System;
using System.Collections.Generic;

namespace ETCRegionManagementSimulator.Events
{
    public class MessageService
    {
        private static readonly MessageService _instance = new MessageService();
        public static MessageService Instance => _instance;
        private readonly Dictionary<string, List<EventHandler<MessageReceivedEventArgs>>> _handlers = new Dictionary<string, List<EventHandler<MessageReceivedEventArgs>>>();

        public void Subscribe(string clientId, EventHandler<MessageReceivedEventArgs> handler)
        {
            if (!_handlers.ContainsKey(clientId))
            {
                _handlers[clientId] = new List<EventHandler<MessageReceivedEventArgs>>();
            }
            _handlers[clientId].Add(handler);
        }

        public void Unsubscribe(string clientId, EventHandler<MessageReceivedEventArgs> handler)
        {
            if (_handlers.ContainsKey(clientId))
            {
                _ = _handlers[clientId].Remove(handler);
                if (_handlers[clientId].Count == 0)
                {
                    _ = _handlers.Remove(clientId);
                }
            }
        }

        public void PublishMessage(string clientId, string message)
        {
            if (_handlers.ContainsKey(clientId))
            {
                MessageReceivedEventArgs args = new MessageReceivedEventArgs(clientId, message);
                foreach (EventHandler<MessageReceivedEventArgs> handler in _handlers[clientId])
                {
                    handler(this, args);
                }
            }
        }
    }
}
