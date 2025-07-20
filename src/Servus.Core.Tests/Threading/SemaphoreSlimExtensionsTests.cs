using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Servus.Core.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Servus.Core.Tests.Threading;

[TestClass]
public class SemaphoreSlimExtensionsTests
{
    [TestMethod]
    public async Task SemaphoreSlimScopeWaitReleaseUsingBlock()
    {
        int count = 0;

        var semaphore = new SemaphoreSlim(1, 1);

        Assert.AreEqual(1, semaphore.CurrentCount);
        using (await semaphore.WaitScopedAsync())
        {
            count++;
            Assert.AreEqual(0, semaphore.CurrentCount);
        }

        Assert.AreEqual(1, count);
        Assert.AreEqual(1, semaphore.CurrentCount);
    }


    [TestMethod]
    [Timeout(30000)]
    public async Task SemaphoreSlimScopeWaitParallelFor()
    {
        const int upperBound = 10000;
        var counter = new CountContainer();
        var items = new List<int>();

        using(var semaphore = new SemaphoreSlim(1, 1))
        {
            //Count to 10000 in paralell and sync the count with a semaphore slim
            var tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(CountAsynct100(semaphore, items, counter));
            }

            await Task.WhenAll(tasks);


            Assert.AreEqual(upperBound, items.Count);

            //Ensure list is ordered from 0 to 9999
            for (int i = 0; i < upperBound - 1; i++)
            {
                Assert.AreEqual(i, items[i]);
            }
        }
    }

    private static async Task CountAsynct100(SemaphoreSlim semaphore, List<int> items, CountContainer counter)
    {
        for (int i = 0; i < 100; i++)
        {
            using (await semaphore.WaitScopedAsync())
            {
                items.Add(counter.Count++);
            }
        }
    }

    [TestMethod]
    [Timeout(30000)]
    public void SemaphoreSlimScopeWait()
    {
        const int upperBound = 10000;
        var counter = new CountContainer();
        var items = new List<int>();

        using (var semaphore = new SemaphoreSlim(1, 1))
        {
            //Count to 10000 in parallel and sync the count with a semaphore slim
            var tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                var task = Task.Run(() =>
                {
                    for (int c = 0; c < 100; c++)
                    {
                        using (semaphore.WaitScoped())
                        {
                            items.Add(counter.Count++);
                        }
                    }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());



            Assert.AreEqual(upperBound, items.Count);

            //Ensure list is ordered from 0 to 9999
            for (int i = 0; i < upperBound - 1; i++)
            {
                Assert.AreEqual(i, items[i]);
            }
        }
    }


    private class CountContainer
    {
        public int Count;
    }
}