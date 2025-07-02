using System.Runtime.CompilerServices;

namespace Servus.Core.Concurrency.Tasks;

public interface IAsyncTask<T> : ITaskMarker
{
    public ValueTask<T> RunAsync(CancellationToken token);
}

public class TaskRegistry<TIn, TOut> : TaskRegistryBase<TIn> where TIn : IAsyncTask<TOut>
{
    public async IAsyncEnumerable<TOut> RunAllAsync(IServiceProvider serviceProvider, [EnumeratorCancellation] CancellationToken token = default)
    {
        foreach (var task in GetStartupTasks(serviceProvider))
        {
            yield return await task.RunAsync(token);
        }
    }
}