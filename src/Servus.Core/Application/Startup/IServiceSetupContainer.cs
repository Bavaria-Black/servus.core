using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Servus.Core.Application.Startup;

public interface IServiceSetupContainer : ISetupContainer
{
    void SetupServices(IServiceCollection services, IConfiguration configuration);
}