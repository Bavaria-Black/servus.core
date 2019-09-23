using System;
using System.Threading;
using System.Threading.Tasks;
using DevTools.Core.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevTools.Core.Tests.Threading
{
    [TestClass]
    public class BlockingTimerTests
    {
        [TestMethod]
        [Timeout(20000)]
        [Ignore]
        public async Task ActionIsFasterThenBlockingTimer()
        {
            //CTS with 2 second timeout
            var cts = new CancellationTokenSource(1500);
            int count = 0;
            var timeoutSemaphore = new SemaphoreSlim(0, int.MaxValue);

            cts.Token.Register(() => timeoutSemaphore.Release());

            var timer = new BlockingTimer(async () =>
            {
                await Task.Delay(10);
                count++;
            }, cts.Token, 600);

            await timeoutSemaphore.WaitAsync();

            Console.WriteLine($@"Count: {count}");
            Assert.IsTrue(cts.IsCancellationRequested);
            Assert.IsTrue(count > 1, $"Count is {count}");
            Assert.IsTrue(count < 3, $"Count is {count}");
        }

        [TestMethod]
        [Ignore]
        [Timeout(40000)]
        public async Task ActionIsSlowerThenBlockingTimer()
        {
            //CTS with 1 second timeout
            int count = 0;
            int interval = 10;
            int runtime = 1000;
            int maxRuns = runtime / interval;
            var cts = new CancellationTokenSource(runtime);
            var semaphore = new SemaphoreSlim(0, 1);

            cts.Token.Register(() =>
            {
                semaphore.Release();
            });

            var timer = new BlockingTimer(async () =>
            {
                await Task.Delay(interval * 2);
                if (count++ > maxRuns)
                {
                    cts.Cancel();
                    Assert.Fail();
                }
            }, cts.Token, interval);

            await semaphore.WaitAsync();

            Console.WriteLine($"Count: {count}/{maxRuns}");
            Assert.IsTrue(cts.IsCancellationRequested);
            Assert.IsTrue(count >= 1, $"Count is {count}/{maxRuns}");
            Assert.IsTrue(count < maxRuns, $"Count is {count}/{maxRuns}");
        }
    }
}
