using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

namespace Servus.Core.Threading.Tasks;

public interface IAsyncTask<T> : ITaskMarker
{
    public ValueTask<T> RunAsync(CancellationToken token);
}

public class TaskRegistry<TIn, TOut> : TaskRegistryBase<TIn> where TIn : IAsyncTask<TOut>
{
    public async IAsyncEnumerable<TOut> RunAllAsync(IServiceProvider serviceProvider, [EnumeratorCancellation] CancellationToken token = default)
    {
        foreach (var task in GetActions(serviceProvider))
        {
            yield return await task.RunAsync(token);
        }
    }
}