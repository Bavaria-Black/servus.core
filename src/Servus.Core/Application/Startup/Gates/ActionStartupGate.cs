namespace Servus.Core.Application.Startup.Gates;

internal sealed class ActionStartupGate(Func<Task<bool>> check) : IStartupGate
{
    public Task<bool> CheckAsync(CancellationToken token = default) => check();
}