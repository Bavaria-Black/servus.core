using System;
using System.Linq;
using System.Threading.Tasks;
using Servus.Core.Threading;

namespace Servus.Core.Events
{
    public class TaskBasedEventBus : EventBus
    {
        public override void Publish<T>(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)} FullName is null.");

            Task.Run(async () =>
            {
                using (await SubscriptionsSemaphore.WaitScopedAsync().ConfigureAwait(false))
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
            });
        }
    }
}