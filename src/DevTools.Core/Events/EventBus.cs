using System;
using System.Collections.Generic;

namespace DevTools.Core.Events
{
    //ToDo: Is the folder events alright?
    //todo:  Publish T?
    //todo: Subscribe to multiple T on same topic
    //todo: Documentation
    //todo: Unsubscribe
    public class EventBus
    {
        private readonly Dictionary<string, Action<object>> _subscriptions = new Dictionary<string, Action<object>>();
        
        public void Publish<T>(string topic, T message)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (_subscriptions.TryGetValue(topic, out var action))
            {
                action(message);
            }
        }

        public void Subscribe<T>(string topic, Action<T> action)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            _subscriptions.Add(topic, message => action((T) message));
        }
    }
}