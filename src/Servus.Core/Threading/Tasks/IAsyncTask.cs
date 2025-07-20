namespace Servus.Core.Threading.Tasks;

public interface IAsyncTask : ITaskMarker
{
    public ValueTask RunAsync(CancellationToken token);
}

public interface IAsyncTask<T> : ITaskMarker
{
    public ValueTask<T> RunAsync(CancellationToken token);
}