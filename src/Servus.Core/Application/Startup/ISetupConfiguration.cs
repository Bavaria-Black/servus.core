using Microsoft.Extensions.Configuration;

namespace Servus.Core.Application.Startup;

public interface ISetupConfiguration
{
    void SetupConfiguration(IConfigurationBuilder builder);
}