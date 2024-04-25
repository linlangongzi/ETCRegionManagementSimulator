using System;
using System.Collections.Generic;

namespace ETCRegionManagementSimulator.Events
{
    public class MessageService
    {
        private readonly object _lock = new object();

        private static readonly MessageService _instance = new MessageService();
        public static MessageService Instance => _instance;
        private readonly Dictionary<string, List<EventHandler<MessageReceivedEventArgs>>> _handlers = new Dictionary<string, List<EventHandler<MessageReceivedEventArgs>>>();

        public void Subscribe(string clientId, EventHandler<MessageReceivedEventArgs> handler)
        {
            lock (_lock)
            {
                if (!_handlers.ContainsKey(clientId))
                {
                    _handlers[clientId] = new List<EventHandler<MessageReceivedEventArgs>>();
                }
                _handlers[clientId].Add(handler);
            }
        }

        public void Unsubscribe(string clientId, EventHandler<MessageReceivedEventArgs> handler)
        {
            lock (_lock)
            {
                if (_handlers.ContainsKey(clientId))
                {
                    _ = _handlers[clientId].Remove(handler);
                    if (_handlers[clientId].Count == 0)
                    {
                        _ = _handlers?.Remove(clientId);
                    }
                }
            }
        }

        public void PublishMessage(string clientId, string message)
        {
            lock(_lock)
            {
                if (_handlers.ContainsKey(clientId))
                {
                    MessageReceivedEventArgs args = new MessageReceivedEventArgs(clientId, message);
                    foreach (EventHandler<MessageReceivedEventArgs> handler in _handlers[clientId])
                    {
                        // Safely invoke the handler if it's not null
                        handler?.Invoke(this, args);
                    }
                }
            }
        }
    }
}
