namespace Servus.Core.Application.Startup.Gates;

public interface IStartupGate
{
    public Task<bool> CheckAsync(CancellationToken token = default);
}

internal sealed class ActionStartupGate(Func<Task<bool>> check) : IStartupGate
{
    public Task<bool> CheckAsync(CancellationToken token = default) => check();
}