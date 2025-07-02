namespace Servus.Core.Threading.Tasks;

public class TaskRegistry<T> : TaskRegistryBase<T> where T : IAsyncTask
{
    public async Task RunAllAsync(IServiceProvider serviceProvider, CancellationToken token = default)
    {
        await RunAllAsync(serviceProvider, f => f.RunAsync(token));
    }
    
    public async Task RunAllAsyncParallel(IServiceProvider serviceProvider, CancellationToken token = default)
    {
        await RunAsyncParallel(serviceProvider, (f, t) => f.RunAsync(t), token);
    }
}