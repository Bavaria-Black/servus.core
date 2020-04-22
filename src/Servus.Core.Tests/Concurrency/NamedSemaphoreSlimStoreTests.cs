using Servus.Core.Concurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Servus.Core.Tests.Concurrency
{
    [TestClass]
    public class NamedSemaphoreSlimStoreTests
    {
        [TestMethod]
        public void OpenOrCreateTest()
        {
            var semaphore = NamedSemaphoreSlimStore.OpenOrCreate("Leberkas");
            Assert.AreEqual(1, semaphore.CurrentCount);
            Assert.AreEqual("Leberkas", semaphore.Name);
            semaphore.Wait();

            var semaphore2 = NamedSemaphoreSlimStore.OpenOrCreate("Leberkas");
            Assert.AreEqual("Leberkas", semaphore2.Name);

            Assert.AreEqual(0, semaphore2.CurrentCount);
            Assert.AreEqual(2, semaphore2.RequestCounter);

            semaphore.Dispose();
            semaphore2.Release();

            Assert.AreEqual(1, semaphore.CurrentCount);
            Assert.AreEqual(1, semaphore.RequestCounter);
            Assert.AreEqual(1, semaphore2.CurrentCount);
            Assert.AreEqual(1, semaphore2.RequestCounter);

            semaphore2.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(semaphore2.Wait);
        }
    }
}
