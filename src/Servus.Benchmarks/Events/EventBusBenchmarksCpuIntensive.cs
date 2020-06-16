using System;
using BenchmarkDotNet.Attributes;
using Servus.Core.Events;

namespace Servus.Benchmarks.Events
{
    [Config(typeof(Config))]
    [RPlotExporter]
    public class Cpu1Sub : EventBusBenchmarksCpuIntensive
    {
        public Cpu1Sub() => ActiveSubscribers = 1;

        // The EventBus is intended as a long living object,
        // so it's creation time is not considered in the benchmark.
        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeCpuHeavyOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TplEventBus);
        }

        [Benchmark]
        public void Sync()
        {
            PublishOperation(SynchronousEventBus);
        }

        [Benchmark]
        public void Tasks()
        {
            PublishOperation(TaskBasedEventBus);
        }

        [Benchmark]
        public void Tpl()
        {
            PublishOperation(TplEventBus);
        }
    }

    [Config(typeof(Config))]
    [RPlotExporter]
    public class Cpu2Sub : EventBusBenchmarksCpuIntensive
    {
        public Cpu2Sub() => ActiveSubscribers = 2;

        // The EventBus is intended as a long living object,
        // so it's creation time is not considered in the benchmark.
        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeCpuHeavyOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TplEventBus);
        }

        [Benchmark]
        public void Sync()
        {
            PublishOperation(SynchronousEventBus);
        }

        [Benchmark]
        public void Tasks()
        {
            PublishOperation(TaskBasedEventBus);
        }

        [Benchmark]
        public void Tpl()
        {
            PublishOperation(TplEventBus);
        }
    }

    [Config(typeof(Config))]
    [RPlotExporter]
    public class Cpu3Sub : EventBusBenchmarksCpuIntensive
    {
        public Cpu3Sub() => ActiveSubscribers = 3;

        // The EventBus is intended as a long living object,
        // so it's creation time is not considered in the benchmark.
        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeCpuHeavyOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TplEventBus);
        }

        [Benchmark]
        public void Sync()
        {
            PublishOperation(SynchronousEventBus);
        }

        [Benchmark]
        public void Tasks()
        {
            PublishOperation(TaskBasedEventBus);
        }

        [Benchmark]
        public void Tpl()
        {
            PublishOperation(TplEventBus);
        }
    }

    [Config(typeof(Config))]
    [RPlotExporter]
    public class Cpu5Sub : EventBusBenchmarksCpuIntensive
    {
        public Cpu5Sub() => ActiveSubscribers = 5;

        // The EventBus is intended as a long living object,
        // so it's creation time is not considered in the benchmark.
        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeCpuHeavyOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TplEventBus);
        }

        [Benchmark]
        public void Sync()
        {
            PublishOperation(SynchronousEventBus);
        }

        [Benchmark]
        public void Tasks()
        {
            PublishOperation(TaskBasedEventBus);
        }

        [Benchmark]
        public void Tpl()
        {
            PublishOperation(TplEventBus);
        }
    }

    [Config(typeof(Config))]
    [RPlotExporter]
    public class Cpu7Sub : EventBusBenchmarksCpuIntensive
    {
        public Cpu7Sub() => ActiveSubscribers = 7;

        // The EventBus is intended as a long living object,
        // so it's creation time is not considered in the benchmark.
        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeCpuHeavyOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TplEventBus);
        }

        [Benchmark]
        public void Sync()
        {
            PublishOperation(SynchronousEventBus);
        }

        [Benchmark]
        public void Tasks()
        {
            PublishOperation(TaskBasedEventBus);
        }

        [Benchmark]
        public void Tpl()
        {
            PublishOperation(TplEventBus);
        }
    }

    [Config(typeof(Config))]
    [RPlotExporter]
    public class Cpu9Sub : EventBusBenchmarksCpuIntensive
    {
        public Cpu9Sub() => ActiveSubscribers = 9;

        // The EventBus is intended as a long living object,
        // so it's creation time is not considered in the benchmark.
        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeCpuHeavyOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TplEventBus);
        }

        [Benchmark]
        public void Sync()
        {
            PublishOperation(SynchronousEventBus);
        }

        [Benchmark]
        public void Tasks()
        {
            PublishOperation(TaskBasedEventBus);
        }

        [Benchmark]
        public void Tpl()
        {
            PublishOperation(TplEventBus);
        }
    }

    [Config(typeof(Config))]
    [RPlotExporter]
    public class Cpu11Sub : EventBusBenchmarksCpuIntensive
    {
        public Cpu11Sub() => ActiveSubscribers = 11;

        // The EventBus is intended as a long living object,
        // so it's creation time is not considered in the benchmark.
        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeCpuHeavyOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeCpuHeavyOperation(TplEventBus);
        }

        [Benchmark]
        public void Sync()
        {
            PublishOperation(SynchronousEventBus);
        }

        [Benchmark]
        public void Tasks()
        {
            PublishOperation(TaskBasedEventBus);
        }

        [Benchmark]
        public void Tpl()
        {
            PublishOperation(TplEventBus);
        }
    }

    /// <summary>
    /// Benchmark base that performs a somewhat cpu heavy operation as subscription action
    /// </summary>
    public abstract class EventBusBenchmarksCpuIntensive : EventBusBenchmarks
    {
        protected void SetupPublishSubscribeCpuHeavyOperation(EventBus eventBus)
        {
            double CpuHeavyOperation(int num)
            {
                var random = new Random(555);
                double result = 1d;

                for (int i = 0; i < 10000; i++)
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
    }
}