using System;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Servus.Core.Events;

namespace Servus.Benchmarks.Events
{
    public abstract class EventBusBenchmarks
    {
        protected const int UnrelatedSubscribers = 1000; 
        protected ManualResetEvent ManualResetEvent;
        protected SynchronousEventBus SynchronousEventBus;
        protected TaskBasedEventBus TaskBasedEventBus;
        protected TplEventBus TplEventBus;
        
        // ToDo: Parameterize the number of topics in the dictionary
        // ToDo: measure sequential vs parallel publish
        // ToDo: Measure Predicates

        [Params(100, 1000, 10000)]
        public int N;
        
        protected int ActiveSubscribers;
        
        [IterationSetup]
        public void Setup() => ManualResetEvent = new ManualResetEvent(false); 
        

        protected void PublishOperation(EventBus eventBus)
        {
            ManualResetEvent = new ManualResetEvent(false);

            // Synchronous publish
            for (int i = 0; i < N; i++)
            {
                eventBus.Publish(new HandledEvent(i));
            }

            // Last event that is published. Because the bus behaves after FIFO principle,
            // it should be done when this event is received.
            eventBus.Publish(new FinishedEvent());
            ManualResetEvent.WaitOne();
        }

        protected class HandledEvent
        {
            public int Number { get; }
            
            public HandledEvent(int number)
            {
                Number = number;
            }
        }
        
        protected class UnhandledEvent
        {
            
        }
        
        protected class FinishedEvent
        {
            
        }
    }
}