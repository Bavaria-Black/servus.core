namespace Servus.Core.Threading.Tasks;

public interface IAsyncTask : ITaskMarker
{
    public ValueTask RunAsync(CancellationToken token);
}