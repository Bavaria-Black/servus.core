using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace DevTools.Core.Events
{
    //ToDo: Is the folder events alright?
    //todo:  Publish T?
    //todo: Subscribe to multiple T on same topic
    //todo: Documentation
    //toDo: Subscribe with CancellationToken Subscribe(() => a(), cancellationToken)
    //toDo: Filter predicate
    //toDo: Benchmark dict vs list
    //toDo: return disposable subscription instead of guid?
    //toDo: Unsubscribe without T?
    //ToDo: Replace T with Event - otherwise everybody could just send a string etc.
    public class EventBus
    {
        private readonly Dictionary<string, List<InternalSubscription>> _subscriptions = new Dictionary<string, List<InternalSubscription>>();
        
        public void Publish<T>(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)} FullName is null.");
            
            if (_subscriptions.TryGetValue(topic, out var subscriptionsForTopic))
            {
                foreach (var subscription in subscriptionsForTopic)
                {
                    // Filter by optional predicate
                    if (subscription.Predicate == null || subscription.Predicate(message))
                    {
                        subscription.Action(message);
                    }
                }
            }
        }

        public Guid Subscribe<T>(Action<T> action, Predicate<T> predicate = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)} FullName is null.");
            
            var internalSubscription = new InternalSubscription(message => action((T) message));
            if (predicate != null)
            {
                internalSubscription.Predicate = o => predicate((T)o);
            }
            
            if (_subscriptions.TryGetValue(topic, out var subscriptionsForTopic))
            {
                subscriptionsForTopic.Add(internalSubscription);
            }
            else
            {
                var newSubscriptionsForTopic = new List<InternalSubscription>() { internalSubscription };
                _subscriptions.Add(topic, newSubscriptionsForTopic);
            }

            return internalSubscription.Id;
        }

        public void Unsubscribe<T>(Guid subscriptionId)
        {
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)} FullName is null.");

            if (_subscriptions.TryGetValue(topic, out var subscriptionsForTopic))
            {
                var index = subscriptionsForTopic.FindIndex(s => s.Id.Equals(subscriptionId));
                if (index > -1)
                {
                    subscriptionsForTopic.RemoveAt(index);
                }
            }
            
            //ToDo: Remove key if list is empty
        }
        
        private class InternalSubscription
        {
            public InternalSubscription(Action<object> action)
            {
                Action = action;
            }

            public Guid Id { get; } = Guid.NewGuid();
            public Action<object> Action { get; }
            public Predicate<object> Predicate { get; internal set; }
        }
    }
    

    // public class Subscription : IDisposable
    // {
    //     
    //     
    // }
}