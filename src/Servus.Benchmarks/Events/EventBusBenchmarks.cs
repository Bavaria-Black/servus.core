using System;
using BenchmarkDotNet.Attributes;
using Servus.Core.Events;

namespace Servus.Benchmarks.Events
{
    [Config(typeof(Config))]
    [RPlotExporter]
    public class EventBusBenchmarks
    {
        private SynchronousEventBus _synchronousEventBus;
        private TaskBasedEventBus _taskBasedEventBus;
        private TplEventBus _tplEventBus;
        
        /*
        Configurable number of:
        Publishers,
        Subscribers,
        BusyWait x,
        NonBusyWait x,
         */
        
        [Params(1000, 10000)]
        public int N;
        
        
        // Create event buses
        // The EventBus is intended as a long living object,
        // so it's creation time is not considered in the benchmark.
        [IterationSetup(Target = nameof(BenchmarkA))]
        public void IterationSetupSynchronousEventBus() => _synchronousEventBus = new SynchronousEventBus();
        
        [IterationSetup(Target = nameof(BenchmarkA))] // toDo: Change benchmark
        public void IterationSetupTaskBasedEventBus() => _taskBasedEventBus = new TaskBasedEventBus();
        
        [IterationSetup(Target = nameof(BenchmarkA))] // toDo: Change benchmark
        public void IterationSetupTplEventBus() =>  _tplEventBus = new TplEventBus();


        [Benchmark]
        public void BenchmarkA()
            => Console.WriteLine("// " + "Benchmark A");
        
        private static void CpuHeavyOperation()
        {
            // ToDo: waste cpu time.
        }
    }
}