using System;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Servus.Core.Events;

namespace Servus.Benchmarks.Events
{
    [Config(typeof(Config))]
    [RPlotExporter]
    public class EventBusBenchmarks
    {
        private ManualResetEvent _manualResetEvent;
        private SynchronousEventBus _synchronousEventBus;
        private TaskBasedEventBus _taskBasedEventBus;
        private TplEventBus _tplEventBus;
        
        // ToDo: Parameterize the number of topics in the dictionary
        // ToDo: measure sequential vs parallel publish
        // ToDo: Measure Predicates
        /*
        Configurable number of:
        Publishers,
        Subscribers,
        BusyWait x,
        NonBusyWait x,
         */
        
        [Params(100, 1000, 10000)]
        public int N;
        
        // [Params(0, 100, 1000, 10000, 100000)]
        [Params(1)]
        public int UnrelatedSubscribers;
        
        [Params(1, 3, 5, 7, 9, 11)]
        public int ActiveSubscribers;

        [Params(10,100,1000)]
        public int OperationModifier;
        

        [IterationSetup]
        public void Setup() => _manualResetEvent = new ManualResetEvent(false); 
        

        # region "CPU Heavy Operation"        // Create event buses
        
        // The EventBus is intended as a long living object,
        // so it's creation time is not considered in the benchmark.
        [IterationSetup(Target = nameof(SynchronousEventBusHeavyOperation))]
        public void SetupSynchronousEventBusHeavyOperation()
        {
            _synchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeCpuHeavyOperation(_synchronousEventBus);
        }

        [IterationSetup(Target = nameof(TaskBasedEventBusHeavyOperation))]
        public void SetupTaskBasedEventBusHeavyOperation()
        {
            _taskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeCpuHeavyOperation(_taskBasedEventBus);
        }

        [IterationSetup(Target = nameof(TplEventBusHeavyOperation))]
        public void SetupTplEventBusHeavyOperation()
        {
            _tplEventBus = new TplEventBus();
            SetupPublishSubscribeCpuHeavyOperation(_tplEventBus);
        }

        private void SetupPublishSubscribeCpuHeavyOperation(EventBus eventBus)
        {
            void CpuHeavyOperation(int num)
            {
                var random = new Random(555);
                double result = 1d;

                for (int i = 0; i < OperationModifier; i++)
                {
                    result = Math.Sqrt(random.NextDouble() + result) * Math.Sqrt(i + num);
                }
            }
            
            for (int i = 0; i < UnrelatedSubscribers; i++)
            {
                eventBus.Subscribe((UnhandledEvent e) => { });    
            }
            
            for (int i = 0; i < ActiveSubscribers; i++)
            {
                eventBus.Subscribe((HandledEvent e) => { CpuHeavyOperation(e.Number); });
            }
            
            eventBus.Subscribe((FinishedEvent e) => _manualResetEvent.Set());
        }

        [Benchmark]
        public void SynchronousEventBusHeavyOperation()
        {
            PublishOperation(_synchronousEventBus);
        }
        
        [Benchmark]
        public void TaskBasedEventBusHeavyOperation()
        {
            PublishOperation(_taskBasedEventBus);
        }
        
        [Benchmark]
        public void TplEventBusHeavyOperation()
        {
            PublishOperation(_tplEventBus);
        }
        
        #endregion
        
        #region "Wait Operation"
        
        [IterationSetup(Target = nameof(SynchronousEventBusWaitOperation))]
        public void SetupSynchronousEventBusWaitOperation()
        {
            _synchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeWaitOperation(_synchronousEventBus);
        }

        [IterationSetup(Target = nameof(TaskBasedEventBusWaitOperation))]
        public void SetupTaskBasedEventBusWaitOperation()
        {
            _taskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeWaitOperation(_taskBasedEventBus);
        }

        [IterationSetup(Target = nameof(TplEventBusWaitOperation))]
        public void SetupTplEventBusWaitOperation()
        {
            _tplEventBus = new TplEventBus();
            SetupPublishSubscribeWaitOperation(_tplEventBus);
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
            
            eventBus.Subscribe((FinishedEvent e) => _manualResetEvent.Set());
        }
        
        [Benchmark]
        public void SynchronousEventBusWaitOperation()
        {
            PublishOperation(_synchronousEventBus);
        }
        
        [Benchmark]
        public void TaskBasedEventBusWaitOperation()
        {
            PublishOperation(_taskBasedEventBus);
        }
        
        [Benchmark]
        public void TplEventBusWaitOperation()
        {
            PublishOperation(_tplEventBus);
        }
        
        #endregion
        
        private void PublishOperation(EventBus eventBus)
        {
            _manualResetEvent = new ManualResetEvent(false);

            // Synchronous publish
            for (int i = 0; i < N; i++)
            {
                eventBus.Publish(new HandledEvent(i));
            }

            // Last event that is published. Because the bus behaves after FIFO principle,
            // it should be done when this event is received.
            eventBus.Publish(new FinishedEvent());
            _manualResetEvent.WaitOne();
        }

        private class HandledEvent
        {
            public int Number { get; }
            
            public HandledEvent(int number)
            {
                Number = number;
            }
        }
        
        private class UnhandledEvent
        {
            
        }
        
        private class FinishedEvent
        {
            
        }
    }
}