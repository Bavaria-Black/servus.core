namespace Servus.Core.Application.Startup.Gates;

public interface IStartupGate
{
    public Task<bool> CheckAsync(CancellationToken token = default);
}