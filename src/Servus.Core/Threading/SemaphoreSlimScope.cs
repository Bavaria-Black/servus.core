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

        #region WaitAsync

        internal static async Task<IDisposable> WaitAsync(SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken = default)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            await semaphoreSlim.WaitAsync(cancellationToken);
            return scope;
        }

        internal static async Task<IDisposable> WaitAsync(SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            if (!await semaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken))
            {
                throw new OperationCanceledException();
            }

            return scope;
        }

        internal static async Task<IDisposable> WaitAsync(SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            if (!await semaphoreSlim.WaitAsync(timeout, cancellationToken))
            {
                throw new OperationCanceledException();
            }

            return scope;
        }

        #endregion

        #region Wait

        internal static IDisposable Wait(SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken = default)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            semaphoreSlim.Wait(cancellationToken);
            return scope;
        }

        internal static IDisposable Wait(SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            if (!semaphoreSlim.Wait(millisecondsTimeout, cancellationToken))
            {
                throw new OperationCanceledException();
            }

            return scope;
        }

        internal static IDisposable Wait(SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            var scope = new SemaphoreSlimScope(semaphoreSlim);
            if (!semaphoreSlim.Wait(timeout, cancellationToken))
            {
                throw new OperationCanceledException();
            }

            return scope;
        }

        #endregion

        public void Dispose()
        {
            _semaphoreSlim.Release();
        }
    }
}
