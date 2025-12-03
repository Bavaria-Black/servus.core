using Microsoft.Extensions.Hosting;

namespace Servus.Core.Application.Startup;

public interface IHostBuilderSetupContainer : ISetupContainer
{
    void ConfigureHostBuilder(IHostBuilder builder);
}