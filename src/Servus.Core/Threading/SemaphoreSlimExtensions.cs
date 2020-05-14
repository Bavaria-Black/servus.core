using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servus.Core.Threading
{
    public static class SemaphoreSlimExtensions
    {
        /// <summary>
        /// Can be used to wait on a semaphore within a using block.
        /// It will be released when the used scope is disposed at the end of the using block.
        /// </summary>
        /// <example>
        /// <code>
        /// var semaphore = new SemaphoreSlim(1,1);
        /// using(await semaphore.WaitScopedAsync())
        /// {
        ///     // the semaphore locks everything inside the using block
        ///     return myUnsafeCall();
        /// } // The semaphore is released here, even if an exception is thrown
        /// </code>
        /// </example>
        public static async Task<IDisposable> WaitScopedAsync(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken = default) 
            => await SemaphoreSlimScope.WaitAsync(semaphoreSlim, cancellationToken);

        /// <summary>
        /// Can be used to wait on a semaphore within a using block.
        /// It will be released when the used scope is disposed at the end of the using block.
        /// </summary>
        /// <example>
        /// <code>
        /// var semaphore = new SemaphoreSlim(1,1);
        /// using(await semaphore.WaitScopedAsync())
        /// {
        ///     // the semaphore locks everything inside the using block
        ///     return myUnsafeCall();
        /// } // The semaphore is released here, even if an exception is thrown
        /// </code>
        /// </example>
        public static async Task<IDisposable> WaitScopedAsync(this SemaphoreSlim semaphoreSlim, int timeoutMilliseconds, CancellationToken cancellationToken = default)
            => await SemaphoreSlimScope.WaitAsync(semaphoreSlim, timeoutMilliseconds, cancellationToken);

        /// <summary>
        /// Can be used to wait on a semaphore within a using block.
        /// It will be released when the used scope is disposed at the end of the using block.
        /// </summary>
        /// <example>
        /// <code>
        /// var semaphore = new SemaphoreSlim(1,1);
        /// using(await semaphore.WaitScopedAsync())
        /// {
        ///     // the semaphore locks everything inside the using block
        ///     return myUnsafeCall();
        /// } // The semaphore is released here, even if an exception is thrown
        /// </code>
        /// </example>
        public static async Task<IDisposable> WaitScopedAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken = default)
            => await SemaphoreSlimScope.WaitAsync(semaphoreSlim, timeout, cancellationToken);

        #region WaitScoped

        /// <summary>
        /// Can be used to wait on a semaphore within a using block.
        /// It will be released when the used scope is disposed at the end of the using block.
        /// </summary>
        /// <example>
        /// <code>
        /// var semaphore = new SemaphoreSlim(1,1);
        /// using(semaphore.WaitScoped())
        /// {
        ///     // the semaphore locks everything inside the using block
        ///     return myUnsafeCall();
        /// } // The semaphore is released here, even if an exception is thrown
        /// </code>
        /// </example>
        public static IDisposable WaitScoped(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken = default)
            => SemaphoreSlimScope.Wait(semaphoreSlim, cancellationToken);

        /// <summary>
        /// Can be used to wait on a semaphore within a using block.
        /// It will be released when the used scope is disposed at the end of the using block.
        /// </summary>
        /// <example>
        /// <code>
        /// var semaphore = new SemaphoreSlim(1,1);
        /// using(semaphore.WaitScoped())
        /// {
        ///     // the semaphore locks everything inside the using block
        ///     return myUnsafeCall();
        /// } // The semaphore is released here, even if an exception is thrown
        /// </code>
        /// </example>
        /// <param name="millisecondsTimeout">Timeout in milliseconds</param>
        public static IDisposable WaitScoped(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken = default)
            => SemaphoreSlimScope.Wait(semaphoreSlim, millisecondsTimeout, cancellationToken);

        /// <summary>
        /// Can be used to wait on a semaphore within a using block.
        /// It will be released when the used scope is disposed at the end of the using block.
        /// </summary>
        /// <example>
        /// <code>
        /// var semaphore = new SemaphoreSlim(1,1);
        /// using(semaphore.WaitScoped())
        /// {
        ///     // the semaphore locks everything inside the using block
        ///     return myUnsafeCall();
        /// } // The semaphore is released here, even if an exception is thrown
        /// </code>
        /// </example>
        /// <param name="timeout">Timeout</param>
        public static IDisposable WaitScoped(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken = default)
            => SemaphoreSlimScope.Wait(semaphoreSlim, timeout, cancellationToken);

        #endregion
    }
}
