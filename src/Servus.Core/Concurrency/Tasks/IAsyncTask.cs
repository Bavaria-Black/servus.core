namespace Servus.Core.Concurrency.Tasks;

public interface IAsyncTask : ITaskMarker
{
    public ValueTask RunAsync(CancellationToken token);
}