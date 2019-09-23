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
        public async Task ActionIsFasterThenBlockingTimer()
        {
            //CTS with 1 second timeout
            var cts = new CancellationTokenSource(1000);
            int count = 0;

            var timer = new BlockingTimer(() => count++, cts.Token, 60);

            await Task.Delay(2000, CancellationToken.None);

            Console.WriteLine($@"Count: {count}");
            Assert.IsTrue(cts.IsCancellationRequested);
            Assert.IsTrue(count > 1, $"Count is {count}");
            Assert.IsTrue(count < 80, $"Count is {count}");
        }

        [TestMethod]
        [Timeout(40000)]
        public async Task ActionIsSlowerThenBlockingTimer()
        {
            //CTS with 1 second timeout
            int count = 0;
            int intervall = 10;
            int runtime = 1000;
            int maxRuns = runtime / intervall;
            var cts = new CancellationTokenSource(runtime);
            var semaphore = new SemaphoreSlim(0, 1);

            cts.Token.Register(() =>
            {
                semaphore.Release();
            });

            var timer = new BlockingTimer(async () =>
            {
                await Task.Delay(intervall * 2);
                if (count++ > maxRuns)
                {
                    cts.Cancel();
                    Assert.Fail();
                }
            }, cts.Token, intervall);

            await semaphore.WaitAsync();

            Console.WriteLine($"Count: {count}/{maxRuns}");
            Assert.IsTrue(cts.IsCancellationRequested);
            Assert.IsTrue(count >= 1, $"Count is {count}/{maxRuns}");
            Assert.IsTrue(count < maxRuns, $"Count is {count}/{maxRuns}");
        }
    }
}
