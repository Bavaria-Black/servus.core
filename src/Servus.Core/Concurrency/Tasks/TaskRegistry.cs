namespace Servus.Core.Concurrency.Tasks;

public class TaskRegistry<T> : TaskRegistryBase<T> where T : IAsyncTask
{
    public async Task RunAllAsync(IServiceProvider serviceProvider, CancellationToken token = default)
    {
        foreach (var task in GetStartupTasks(serviceProvider))
        {
            await task.RunAsync(token);
        }
    }
    
    public async Task ParallelRunAllAsync(IServiceProvider serviceProvider, CancellationToken token = default)
    {
        await Parallel
            .ForEachAsync(
                source: GetStartupTasks(serviceProvider),
                cancellationToken: token,
                body: (task, cancellationToken) => task.RunAsync(cancellationToken));
    }
}