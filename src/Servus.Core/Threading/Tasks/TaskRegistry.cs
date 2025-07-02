namespace Servus.Core.Threading.Tasks;

/// <summary>
/// A specialized task registry that manages and executes asynchronous tasks that implement IAsyncTask.
/// </summary>
/// <typeparam name="T">The task type that must implement IAsyncTask.</typeparam>

public class TaskRegistry<T> : TaskRegistryBase<T> where T : IAsyncTask
{
    /// <summary>
    /// Executes all registered tasks asynchronously in sequence.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve registered task types.</param>
    /// <param name="token">The cancellation token to cancel the task execution. Default is default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous execution of all registered tasks.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a registered task type cannot be resolved from the service provider.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <remarks>
    /// Tasks are executed sequentially, one after another, until all tasks complete or an exception occurs.
    /// </remarks>
    public async Task RunAllAsync(IServiceProvider serviceProvider, CancellationToken token = default)
    {
        await RunAllAsync(serviceProvider, f => f.RunAsync(token));
    }
    
    /// <summary>
    /// Executes all registered tasks asynchronously in parallel.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve registered task types.</param>
    /// <param name="token">The cancellation token to cancel the task execution. Default is default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous parallel execution of all registered tasks.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a registered task type cannot be resolved from the service provider.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <remarks>
    /// All tasks are executed concurrently and the method completes when all tasks have finished or when cancelled.
    /// </remarks>
    public async Task RunAllAsyncParallel(IServiceProvider serviceProvider, CancellationToken token = default)
    {
        await RunAsyncParallel(serviceProvider, (f, t) => f.RunAsync(t), token);
    }
}