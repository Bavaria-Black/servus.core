namespace Servus.Core.Threading;

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
    public static async Task<IDisposable> WaitScopedAsync(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken = default(CancellationToken)) => await SemaphoreSlimScope.WaitAsync(semaphoreSlim, cancellationToken);

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
    public static IDisposable WaitScoped(this SemaphoreSlim semaphoreSlim) => SemaphoreSlimScope.Wait(semaphoreSlim);
}