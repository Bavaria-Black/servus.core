using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Servus.Core.Events;

namespace Servus.Benchmarks.Events
{
    [Config(typeof(Config))]
    [RPlotExporter]
    public class Wait1Sub : EventBusBenchmarksWait
    {
        public Wait1Sub() => ActiveSubscribers = 1;

        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeWaitOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeWaitOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeWaitOperation(TplEventBus);
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
    public class Wait2Sub : EventBusBenchmarksWait
    {
        public Wait2Sub() => ActiveSubscribers = 2;

        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeWaitOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeWaitOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeWaitOperation(TplEventBus);
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
    public class Wait3Sub : EventBusBenchmarksWait
    {
        public Wait3Sub() => ActiveSubscribers = 3;

        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeWaitOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeWaitOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeWaitOperation(TplEventBus);
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
    public class Wait5Sub : EventBusBenchmarksWait
    {
        public Wait5Sub() => ActiveSubscribers = 5;

        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeWaitOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeWaitOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeWaitOperation(TplEventBus);
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
    public class Wait7Sub : EventBusBenchmarksWait
    {
        public Wait7Sub() => ActiveSubscribers = 7;

        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeWaitOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeWaitOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeWaitOperation(TplEventBus);
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
    public class Wait9Sub : EventBusBenchmarksWait
    {
        public Wait9Sub() => ActiveSubscribers = 9;

        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeWaitOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeWaitOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeWaitOperation(TplEventBus);
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
    public class Wait11Sub : EventBusBenchmarksWait
    {
        public Wait11Sub() => ActiveSubscribers = 11;

        [IterationSetup(Target = nameof(Sync))]
        public void SetupSync()
        {
            SynchronousEventBus = new SynchronousEventBus();
            SetupPublishSubscribeWaitOperation(SynchronousEventBus);
        }

        [IterationSetup(Target = nameof(Tasks))]
        public void SetupTasks()
        {
            TaskBasedEventBus = new TaskBasedEventBus();
            SetupPublishSubscribeWaitOperation(TaskBasedEventBus);
        }

        [IterationSetup(Target = nameof(Tpl))]
        public void SetupTpl()
        {
            TplEventBus = new TplEventBus();
            SetupPublishSubscribeWaitOperation(TplEventBus);
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

    public abstract class EventBusBenchmarksWait : EventBusBenchmarks
    {
        protected void SetupPublishSubscribeWaitOperation(EventBus eventBus)
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
    }
}