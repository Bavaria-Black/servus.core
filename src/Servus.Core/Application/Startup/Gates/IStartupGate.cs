namespace Servus.Core.Application.Startup.Tasks;

public interface IStartupGate
{
    public ValueTask CheckAsync(CancellationToken token = default);
}