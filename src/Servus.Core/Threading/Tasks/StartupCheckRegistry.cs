using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

namespace Servus.Core.Threading.Tasks;

public interface IAsyncTask<T> : ITaskMarker
{
    public ValueTask<T> RunAsync(CancellationToken token);
}

public class TaskRegistry<TIn, TOut> : TaskRegistryBase<TIn> where TIn : IAsyncTask<TOut>
{
    /// <summary>
    /// Executes all registered tasks asynchronously and yields their results as they complete.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve registered task types.</param>
    /// <param name="token">The cancellation token to cancel the task execution. Default is default(CancellationToken).</param>
    /// <returns>An async enumerable that yields the results of each executed task as they complete.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a registered task type cannot be resolved from the service provider.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <remarks>
    /// Tasks are executed sequentially, and results are yielded as each task completes. 
    /// The EnumeratorCancellation attribute ensures proper cancellation token propagation through the async enumerable.
    /// </remarks>

    public async IAsyncEnumerable<TOut> RunAllAsync(IServiceProvider serviceProvider, [EnumeratorCancellation] CancellationToken token = default)
    {
        foreach (var task in GetActions(serviceProvider))
        {
            yield return await task.RunAsync(token);
        }
    }
}