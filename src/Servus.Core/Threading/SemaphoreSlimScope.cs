using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servus.Core.Threading
{
    /// <summary>
    /// Wraps a <see cref="SemaphoreSlim"/> into a <see cref="IDisposable"/>.
    /// The SemaphoreSlimScope is used by <see cref="SemaphoreSlimExtensions"/>
    /// to lock everything within a using block.
    /// </summary>
    internal sealed class SemaphoreSlimScope : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        private SemaphoreSlimScope(SemaphoreSlim semaphoreSlim)
        {
            _semaphoreSlim = semaphoreSlim;
        }

        internal static async Task<IDisposable> WaitAsync(SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken = default(CancellationToken))
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            await semaphoreSlim.WaitAsync(cancellationToken);
            return scope;
        }

        internal static IDisposable Wait(SemaphoreSlim semaphoreSlim)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            semaphoreSlim.Wait();
            return scope;
        }

        public void Dispose()
        {
            _semaphoreSlim.Release();
        }
    }
}
