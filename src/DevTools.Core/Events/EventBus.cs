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
        private readonly Dictionary<string, List<Action<object>>> _subscriptions = new Dictionary<string, List<Action<object>>>();
        
        public void Publish<T>(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            
            var topic = typeof(T).FullName;
            
            if (_subscriptions.TryGetValue(topic, out var actions))
            {
                foreach (var action in actions)
                {
                    action(message);
                }
            }
        }

        public void Subscribe<T>(Action<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            var topic = typeof(T).FullName;

            if (_subscriptions.TryGetValue(topic, out var actions))
            {
                actions.Add( message => action((T) message));
            }
            else
            {
                var newActions = new List<Action<object>>() {message => action((T) message)};
                _subscriptions.Add(topic, newActions);
            }

        }
    }
}