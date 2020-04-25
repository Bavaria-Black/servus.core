using System;
using System.Linq;
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
                        // Execute subscription action
                        subscription.Action(message);
                    }
                }
            }
        }
    }
}