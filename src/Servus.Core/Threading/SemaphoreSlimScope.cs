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

        internal static IDisposable Wait(SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            semaphoreSlim.Wait(cancellationToken);
            return scope;
        }

        internal static IDisposable Wait(SemaphoreSlim semaphoreSlim, int millisecondsTimeout)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            if (!semaphoreSlim.Wait(millisecondsTimeout))
            {
                throw new OperationCanceledException();
            }
            return scope;
        }

        internal static IDisposable Wait(SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            if (!semaphoreSlim.Wait(millisecondsTimeout, cancellationToken))
            {
                throw new OperationCanceledException();
            }
            return scope;
        }

        internal static IDisposable Wait(SemaphoreSlim semaphoreSlim, TimeSpan timeout)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            if (!semaphoreSlim.Wait(timeout))
            {
                throw new OperationCanceledException();
            }
            return scope;
        }

        internal static IDisposable Wait(SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            if (!semaphoreSlim.Wait(timeout, cancellationToken))
            {
                throw new OperationCanceledException();
            }
            return scope;
        }

        public void Dispose()
        {
            _semaphoreSlim.Release();
        }
    }
}
