namespace Servus.Core.Threading.Tasks;

public interface IActionRegistryRunner<out T>
{
    /// <summary>
    /// Executes all registered actions synchronously using the provided executor function.
    /// </summary>
    /// <param name="sp">The service provider used to resolve registered types.</param>
    /// <param name="executor">The function that defines how each action should be executed.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the parallel execution.</param>
    /// <exception cref="InvalidOperationException">Thrown when a registered type cannot be resolved from the service provider.</exception>
    void RunAll(IServiceProvider sp, Action<T, CancellationToken> executor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes all registered actions asynchronously in parallel using the provided executor function.
    /// </summary>
    /// <param name="sp">The service provider used to resolve registered types.</param>
    /// <param name="executor">The async function that defines how each action should be executed.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the parallel execution.</param>
    /// <returns>A ValueTask representing the asynchronous parallel execution of all actions.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a registered type cannot be resolved from the service provider.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    ValueTask RunAsyncParallel(IServiceProvider sp, Func<T, CancellationToken, ValueTask> executor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes all registered actions asynchronously in sequence using the provided executor function.
    /// </summary>
    /// <param name="sp">The service provider used to resolve registered types.</param>
    /// <param name="executor">The async function that defines how each action should be executed.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the parallel execution.</param>
    /// <returns>A ValueTask representing the asynchronous sequential execution of all actions.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a registered type cannot be resolved from the service provider.</exception>
    ValueTask RunAllAsync(IServiceProvider sp, Func<T, CancellationToken, ValueTask> executor, CancellationToken cancellationToken = default);
}