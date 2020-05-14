using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Servus.Core.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Servus.Core.Tests.Threading
{
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

            using (var semaphore = new SemaphoreSlim(1, 1))
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

        #region WaitScoped

        [TestMethod]
        [Timeout(1000)]
        public async Task WaitScopedWithCancellationToken()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;

            var task = Task.Run(() =>
            {
                try
                {
                    using (semaphore.WaitScoped(cts.Token))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            await Task.Delay(50);

            cts.Cancel();

            await task;

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public void WaitScopedWithTimeoutMilliseconds()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            var waitSuccessfull = false;

            using (semaphore.WaitScoped(100))
            {
                waitSuccessfull = true;
            }

            Assert.IsTrue(waitSuccessfull);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedWithTimeoutMillisecondsFails()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;

            await Task.Run(() =>
            {
                try
                {
                    using (semaphore.WaitScoped(100))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public void WaitScopedWithTimeoutMillisecondsAndCancellationToken()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;

            using (semaphore.WaitScoped(100, cts.Token))
            {
                waitSuccessfull = true;
            }

            Assert.IsTrue(waitSuccessfull);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedWithTimeoutMillisecondsAndCancellationTokenFailsByTimeout()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;

            await Task.Run(() =>
            {
                try
                {
                    using (semaphore.WaitScoped(100, cts.Token))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedWithTimeoutMillisecondsAndCancellationTokenFailsByCancellationToken()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;

            var task = Task.Run(() =>
            {
                try
                {
                    using (semaphore.WaitScoped(100, cts.Token))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            cts.Cancel();

            await task;

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public void WaitScopedWithTimeoutTimeSpan()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            var waitSuccessfull = false;
            var timeout = TimeSpan.FromMilliseconds(100);

            using (semaphore.WaitScoped(timeout))
            {
                waitSuccessfull = true;
            }

            Assert.IsTrue(waitSuccessfull);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedWithTimeoutTimeSpanFails()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;
            var timeout = TimeSpan.FromMilliseconds(100);

            await Task.Run(() =>
            {
                try
                {
                    using (semaphore.WaitScoped(timeout))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public void WaitScopedWithTimeoutTimeSpanAndCancellationToken()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var timeout = TimeSpan.FromMilliseconds(100);

            using (semaphore.WaitScoped(timeout, cts.Token))
            {
                waitSuccessfull = true;
            }

            Assert.IsTrue(waitSuccessfull);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedWithTimeoutTimeSpanAndCancellationTokenFailsByTimeout()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;
            var timeout = TimeSpan.FromMilliseconds(100);

            await Task.Run(() =>
            {
                try
                {
                    using (semaphore.WaitScoped(timeout, cts.Token))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedWithTimeoutTimeSpanAndCancellationTokenFailsByCancellationToken()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;
            var timeout = TimeSpan.FromMilliseconds(100);

            var task = Task.Run(() =>
            {
                try
                {
                    using (semaphore.WaitScoped(timeout, cts.Token))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            cts.Cancel();

            await task;

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        #endregion

        #region WaitScopedAsync


        [TestMethod]
        [Timeout(1000)]
        public async Task WaitScopedAsyncWithCancellationToken()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;

            var task = Task.Run(async () =>
            {
                try
                {
                    using (await semaphore.WaitScopedAsync(cts.Token))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            await Task.Delay(50);

            cts.Cancel();

            await task;

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedAsyncWithTimeoutMilliseconds()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            var waitSuccessfull = false;

            using (await semaphore.WaitScopedAsync(100))
            {
                waitSuccessfull = true;
            }

            Assert.IsTrue(waitSuccessfull);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedAsyncWithTimeoutMillisecondsFails()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;

            await Task.Run(async () =>
            {
                try
                {
                    using (await semaphore.WaitScopedAsync(100))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedAsyncWithTimeoutMillisecondsAndCancellationToken()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;

            using (await semaphore.WaitScopedAsync(100, cts.Token))
            {
                waitSuccessfull = true;
            }

            Assert.IsTrue(waitSuccessfull);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedAsyncWithTimeoutMillisecondsAndCancellationTokenFailsByTimeout()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;

            await Task.Run(async () =>
            {
                try
                {
                    using (await semaphore.WaitScopedAsync(100, cts.Token))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedAsyncWithTimeoutMillisecondsAndCancellationTokenFailsByCancellationToken()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;

            var task = Task.Run(async () =>
            {
                try
                {
                    using (await semaphore.WaitScopedAsync(100, cts.Token))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            cts.Cancel();

            await task;

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedAsyncWithTimeoutTimeSpan()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            var waitSuccessfull = false;
            var timeout = TimeSpan.FromMilliseconds(100);

            using (await semaphore.WaitScopedAsync(timeout))
            {
                waitSuccessfull = true;
            }

            Assert.IsTrue(waitSuccessfull);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedAsyncWithTimeoutTimeSpanFails()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;
            var timeout = TimeSpan.FromMilliseconds(100);

            await Task.Run(async () =>
            {
                try
                {
                    using (await semaphore.WaitScopedAsync(timeout))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedAsyncWithTimeoutTimeSpanAndCancellationToken()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var timeout = TimeSpan.FromMilliseconds(100);

            using (await semaphore.WaitScopedAsync(timeout, cts.Token))
            {
                waitSuccessfull = true;
            }

            Assert.IsTrue(waitSuccessfull);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedAsyncWithTimeoutTimeSpanAndCancellationTokenFailsByTimeout()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;
            var timeout = TimeSpan.FromMilliseconds(100);

            await Task.Run(async () =>
            {
                try
                {
                    using (await semaphore.WaitScopedAsync(timeout, cts.Token))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        [TestMethod]
        [Timeout(300)]
        public async Task WaitScopedAsyncWithTimeoutTimeSpanAndCancellationTokenFailsByCancellationToken()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var cts = new CancellationTokenSource();
            var waitSuccessfull = false;
            var operationCanceledExceptionThrown = false;
            var timeout = TimeSpan.FromMilliseconds(100);

            var task = Task.Run(async () =>
            {
                try
                {
                    using (await semaphore.WaitScopedAsync(timeout, cts.Token))
                    {
                        waitSuccessfull = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    operationCanceledExceptionThrown = true;
                }
            });

            cts.Cancel();

            await task;

            Assert.IsFalse(waitSuccessfull);
            Assert.IsTrue(operationCanceledExceptionThrown);
        }

        #endregion

        private class CountContainer
        {
            public int Count;
        }
    }
}
