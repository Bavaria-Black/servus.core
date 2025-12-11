using Microsoft.Extensions.Hosting;

namespace Servus.Core.Application.Startup;

public interface IHostApplicationBuilderSetupContainer : ISetupContainer
{
    void ConfigureHostApplicationBuilder(IHostApplicationBuilder builder);
}