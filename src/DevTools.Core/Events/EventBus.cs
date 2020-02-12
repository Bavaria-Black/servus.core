using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace DevTools.Core.Events
{
    //ToDo: Is the folder events alright?
    //todo: Publish T?
    //todo: Documentation
    //toDo: Subscribe with CancellationToken Subscribe(() => a(), cancellationToken)
    //toDo: Benchmark dict vs list
    //toDo: return disposable subscription instead of guid? As alternative to the subscription manager.
    //toDo: Unsubscribe without T? Optional at least.
    //ToDo: Replace T with Event - otherwise everybody could just send a string etc.
    //ToDo: Subscription action. Trigger with Task.Run? Or from separate Thread? Or multiple threads? Or TPL?
    public class EventBus
    {
        private readonly Dictionary<string, List<InternalSubscription>> _subscriptions = new Dictionary<string, List<InternalSubscription>>();
        private readonly ActionBlock<(string topic, object message)> _publishActionBlock;

        public EventBus()
        {
            _publishActionBlock = new ActionBlock<(string topic, object message)>(t =>
            {
                // ToDo: Synchronize access to _subscriptions
                Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
                if (_subscriptions.TryGetValue(t.topic, out var subscriptionsForTopic))
                {
                    foreach (var subscription in subscriptionsForTopic)
                    {
                        // Filter by optional predicate
                        if (subscription.Predicate == null || subscription.Predicate(t.message))
                        {
                            subscription.Action(t.message);
                        }
                    }
                }
            });
            // new ExecutionDataflowBlockOptions(){ MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded}
        }
        
        
        public void Publish<T>(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)} FullName is null.");

            _publishActionBlock.Post((topic, message));
        }
        
        // Subscribes synchronously (waits until subscription is made)
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
                
                // Remove key if list is empty
                if (subscriptionsForTopic.Count == 0)
                {
                    _subscriptions.Remove(topic);
                }
            }
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