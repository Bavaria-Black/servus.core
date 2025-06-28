using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Servus.Core.Application;

public abstract class AppConfigurationBase
{
    public abstract void SetupServices(IServiceCollection serviceCollection, IConfiguration configuration);
}