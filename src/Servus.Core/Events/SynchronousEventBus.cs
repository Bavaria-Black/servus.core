using System;
using System.Linq;
using System.Threading.Tasks;
using Servus.Core.Threading;

namespace Servus.Core.Events
{
    public class SynchronousEventBus : EventBus
    {
        public override void Publish<T>(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)} FullName is null.");

            using (SubscriptionsSemaphore.WaitScoped())
            {
                if (Subscriptions.TryGetValue(topic, out var subscriptionsForTopic))
                {
                    // Filter by optional predicate
                    foreach (var subscription in subscriptionsForTopic.Where(subscription => subscription.Predicate == null || subscription.Predicate(message)))
                    {
                        // Execute subscription action / async fallback
                        subscription.Action(message);
                    }
                }
            }
        }
        
        public async Task PublishAsync<T>(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)} FullName is null.");

            using (await SubscriptionsSemaphore.WaitScopedAsync().ConfigureAwait(false))
            {
                if (Subscriptions.TryGetValue(topic, out var subscriptionsForTopic))
                {
                    // Filter by optional predicate
                    foreach (var subscription in subscriptionsForTopic.Where(subscription => subscription.Predicate == null || subscription.Predicate(message)))
                    {
                        if (subscription.IsAsync)
                        {
                            // Execute async subscription func
                            await subscription.AsyncFunc(message).ConfigureAwait(false);
                        }
                        else
                        {
                            // Execute sync subscription action
                            subscription.Action(message);    
                        }
                    }
                }
            }
        }
    }
}