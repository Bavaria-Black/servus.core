using Servus.Core.Concurrency;
using Xunit;
using System;

namespace Servus.Core.Tests.Concurrency;

public class NamedSemaphoreSlimStoreTests
{
    [Fact]
    public void OpenOrCreateTest()
    {
        var semaphore = NamedSemaphoreSlimStore.OpenOrCreate("Leberkas");
        Assert.Equal(1, semaphore.CurrentCount);
        Assert.Equal("Leberkas", semaphore.Name);
        semaphore.Wait();

        var semaphore2 = NamedSemaphoreSlimStore.OpenOrCreate("Leberkas");
        Assert.Equal("Leberkas", semaphore2.Name);

        Assert.Equal(0, semaphore2.CurrentCount);
        Assert.Equal(2, semaphore2.RequestCounter);

        semaphore.Dispose();
        semaphore2.Release();

        Assert.Equal(1, semaphore.CurrentCount);
        Assert.Equal(1, semaphore.RequestCounter);
        Assert.Equal(1, semaphore2.CurrentCount);
        Assert.Equal(1, semaphore2.RequestCounter);

        semaphore2.Dispose();
        Assert.Throws<ObjectDisposedException>(semaphore2.Wait);
    }
}