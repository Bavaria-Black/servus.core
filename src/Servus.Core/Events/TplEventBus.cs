using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Servus.Core.Threading;

namespace Servus.Core.Events
{

    public class TplEventBus : EventBus
    {
        private readonly TransformManyBlock<(string topic, object message), Action> _findSubscribersBlock;
        private readonly ActionBlock<Action> _publishActionBlock;
        
        public TplEventBus()
        {
            _findSubscribersBlock = new TransformManyBlock<(string topic, object message), Action>(args =>
            {
                var actions = new List<Action>();
                
                Debug.WriteLine("_findSubscribersBlock " + Thread.CurrentThread.ManagedThreadId);
                using (SubscriptionsSemaphore.WaitScoped())
                {
                    if (Subscriptions.TryGetValue(args.topic, out var subscriptionsForTopic))
                    {
                        foreach (var subscription in subscriptionsForTopic)
                        {
                            // Filter by optional predicate
                            if (subscription.Predicate == null || subscription.Predicate(args.message))
                            {
                                actions.Add(() => subscription.Action(args.message));
                            }
                        }
                    }
                }
                
                // ToDo: Yield return beneficial or not?
                return actions;
            });
            
            _publishActionBlock = new ActionBlock<Action>(publishAction =>
            {
                Debug.WriteLine("_publishActionBlock " + Thread.CurrentThread.ManagedThreadId);
                publishAction();
            }, new ExecutionDataflowBlockOptions(){ MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded});
            
            // Connect the data flow blocks to form a pipeline.
            _findSubscribersBlock.LinkTo(_publishActionBlock);
        }
        public override void Publish<T>(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var topic = typeof(T).FullName;    
            if(topic == null) throw new ArgumentNullException($"Type parameter {nameof(T)}.FullName is null.");

            _findSubscribersBlock.Post((topic, message));
        }
    }
}