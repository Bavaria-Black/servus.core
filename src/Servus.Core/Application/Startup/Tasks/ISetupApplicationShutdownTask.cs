namespace Servus.Core.Application.Startup.Tasks;

public interface ISetupApplicationShutdownTask
{
    void OnApplicationShuttingDown(IServiceProvider serviceProvider);
}