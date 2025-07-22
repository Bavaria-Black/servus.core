using Microsoft.Extensions.Configuration;

namespace Servus.Core.Application.Startup;

public interface IConfigurationSetupContainer : ISetupContainer
{
    void SetupConfiguration(IConfigurationManager builder);
}