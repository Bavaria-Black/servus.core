using System;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Servus.Core.Events;

namespace Servus.Benchmarks.Events
{
    public class EventBusBenchmarksCpuIntensive : EventBusBenchmarks
    {
        
        // The EventBus is intended as a long living object,
        // so it's creation time is not considered in the benchmark.
        [IterationSetup(Target = nameof(SynchronousEventBusHeavyOperation))]
        public void SetupSynchronousEventBusHeavyOperation()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeCpuHeavyOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(TaskBasedEventBusHeavyOperation))]
        public void SetupTaskBasedEventBusHeavyOperation()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(TplEventBusHeavyOperation))]
        public void SetupTplEventBusHeavyOperation()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TplEventBus);
        }

        private void SetupPublishSubscribeCpuHeavyOperation(EventBus eventBus)
        {
            double CpuHeavyOperation(int num)
            {
                var random = new Random(555);
                double result = 1d;

                for (int i = 0; i < OperationModifier; i++)
                {
                    result = Math.Sqrt(random.NextDouble() + result) * Math.Sqrt(i + num);
                }

                return result;
            }
            
            for (int i = 0; i < UnrelatedSubscribers; i++)
            {
                eventBus.Subscribe((UnhandledEvent e) => { });    
            }
            
            for (int i = 0; i < ActiveSubscribers; i++)
            {
                eventBus.Subscribe((HandledEvent e) => { CpuHeavyOperation(e.Number); });
            }
            
            eventBus.Subscribe((FinishedEvent e) => ManualResetEvent.Set());
        }

        [Benchmark]
        public void SynchronousEventBusHeavyOperation()
        {
            PublishOperation(SynchronousEventBus);
        }
        
        [Benchmark]
        public void TaskBasedEventBusHeavyOperation()
        {
            PublishOperation(TaskBasedEventBus);
        }
        
        [Benchmark]
        public void TplEventBusHeavyOperation()
        {
            PublishOperation(TplEventBus);
        }
    }
    
    [Config(typeof(Config))]
    [RPlotExporter]
    public class EventBusBenchmarksWait : EventBusBenchmarks
    {
        
        [IterationSetup(Target = nameof(SynchronousEventBusWaitOperation))]
        public void SetupSynchronousEventBusWaitOperation()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeWaitOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(TaskBasedEventBusWaitOperation))]
        public void SetupTaskBasedEventBusWaitOperation()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeWaitOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(TplEventBusWaitOperation))]
        public void SetupTplEventBusWaitOperation()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeWaitOperation(TplEventBus);
        }
        
        private void SetupPublishSubscribeWaitOperation(EventBus eventBus)
        {
            for (int i = 0; i < UnrelatedSubscribers; i++)
            {
                eventBus.Subscribe((UnhandledEvent e) => { });    
            }
            
            for (int i = 0; i < ActiveSubscribers; i++)
            {
                eventBus.Subscribe((HandledEvent e) => Task.Delay(OperationModifier / 10));
            }
            
            eventBus.Subscribe((FinishedEvent e) => ManualResetEvent.Set());
        }
        
        [Benchmark]
        public void SynchronousEventBusWaitOperation()
        {
            PublishOperation(SynchronousEventBus);
        }
        
        [Benchmark]
        public void TaskBasedEventBusWaitOperation()
        {
            PublishOperation(TaskBasedEventBus);
        }
        
        [Benchmark]
        public void TplEventBusWaitOperation()
        {
            PublishOperation(TplEventBus);
        }
    }
    

    public class EventBusBenchmarks
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

        // [Params(1, 3, 5, 7, 9, 11)]
        public int ActiveSubscribers = 1;

        // [Params(10, 100, 1000)]
        public int OperationModifier = 10000;
        

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