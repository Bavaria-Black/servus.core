using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Servus.Core.Threading;

namespace Servus.Core.Events
{
    //todo: Documentation
    //toDo: Subscribe with CancellationToken Subscribe(() => a(), cancellationToken)
    //toDo: return disposable subscription instead of guid? As alternative to the subscription manager.
    //ToDo: Replace T with Event - otherwise everybody could just send a string etc.
    //ToDo: Subscription action. Trigger with Task.Run? Or from separate Thread? Or multiple threads? Or TPL?
    //ToDo: Dispose and stop pipeline.
    //ToDo: Interface and a simpler synchronous version of the event bus?
    //ToDo: Benchmark async semaphore vs non async one
    //ToDo: Subscribe and get notified via a async action
    public abstract class EventBus
    {
        protected readonly Dictionary<string, List<InternalSubscription>> Subscriptions = new Dictionary<string, List<InternalSubscription>>();
        protected readonly SemaphoreSlim SubscriptionsSemaphore = new SemaphoreSlim(1,1);

        public abstract void Publish<T>(T message);
        
        
        public Guid Subscribe<T>(Action<T> action, Predicate<T> predicate = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)}.FullName is null.");
            
            var internalSubscription = new InternalSubscription(message => action((T) message));
            if (predicate != null)
            {
                internalSubscription.Predicate = o => predicate((T)o);
            }

            using (SubscriptionsSemaphore.WaitScoped())
            {
                AddSubscription<T>(topic, internalSubscription);
            }

            return internalSubscription.Id;
        }
        
        public async Task<Guid> SubscribeAsync<T>(Action<T> action, Predicate<T> predicate = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)}.FullName is null.");
            
            var internalSubscription = new InternalSubscription(message => action((T) message));
            if (predicate != null)
            {
                internalSubscription.Predicate = o => predicate((T)o);
            }

            using (await SubscriptionsSemaphore.WaitScopedAsync().ConfigureAwait(false))
            {
                AddSubscription<T>(topic, internalSubscription);
            }

            return internalSubscription.Id;
        }

        public Guid Subscribe<T>(Func<T, Task> func, Predicate<T> predicate = null)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)}.FullName is null.");
            
            var internalSubscription = new InternalSubscription(message => func((T) message),
                async message => await func((T) message).ConfigureAwait(false));
            if (predicate != null)
            {
                internalSubscription.Predicate = o => predicate((T)o);
            }

            using (SubscriptionsSemaphore.WaitScoped())
            {
                AddSubscription<T>(topic, internalSubscription);
            }
            
            return internalSubscription.Id;
        }
        
        public async Task<Guid> SubscribeAsync<T>(Func<T, Task> func, Predicate<T> predicate = null)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)}.FullName is null.");
            
            var internalSubscription = new InternalSubscription(message => func((T) message),
                async message => await func((T) message).ConfigureAwait(false));
            if (predicate != null)
            {
                internalSubscription.Predicate = o => predicate((T)o);
            }

            using (await SubscriptionsSemaphore.WaitScopedAsync().ConfigureAwait(false))
            {
                AddSubscription<T>(topic, internalSubscription);
            }
            
            return internalSubscription.Id;
        }

        private void AddSubscription<T>(string topic, InternalSubscription internalSubscription)
        {
            if (Subscriptions.TryGetValue(topic, out var subscriptionsForTopic))
            {
                subscriptionsForTopic.Add(internalSubscription);
            }
            else
            {
                var newSubscriptionsForTopic = new List<InternalSubscription>() {internalSubscription};
                Subscriptions.Add(topic, newSubscriptionsForTopic);
            }
        }

        public void Unsubscribe<T>(Guid subscriptionId)
        {
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)}.FullName is null.");

            using (SubscriptionsSemaphore.WaitScoped())
            {
                RemoveSubscription(subscriptionId, topic);
            }
        }
        
        public async Task UnsubscribeAsync<T>(Guid subscriptionId)
        {
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)}.FullName is null.");

            using (await SubscriptionsSemaphore.WaitScopedAsync().ConfigureAwait(false))
            {
                RemoveSubscription(subscriptionId, topic);
            }
        }

        private void RemoveSubscription(Guid subscriptionId, string topic)
        {
            if (Subscriptions.TryGetValue(topic, out var subscriptionsForTopic))
            {
                var index = subscriptionsForTopic.FindIndex(s => s.Id.Equals(subscriptionId));
                if (index > -1)
                {
                    subscriptionsForTopic.RemoveAt(index);
                }
                    
                // Remove key if list is empty
                if (subscriptionsForTopic.Count == 0)
                {
                    Subscriptions.Remove(topic);
                }
            }
        }

        protected class InternalSubscription
        {
            public InternalSubscription(Action<object> action)
            {
                Action = action;
                IsAsync = false;
            }
            
            public InternalSubscription(Action<object> syncActionFallback, Func<object, Task> asyncFunc)
            {
                Action = syncActionFallback;
                AsyncFunc = asyncFunc;
                IsAsync = true;
            }

            public bool IsAsync { get; }
            public Guid Id { get; } = Guid.NewGuid();
            public Action<object> Action { get; }
            public Func<object, Task> AsyncFunc { get; }
            public Predicate<object> Predicate { get; internal set; }
        }
    }
}