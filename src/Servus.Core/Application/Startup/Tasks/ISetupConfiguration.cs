using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Servus.Core.Application.Startup.Tasks;

public interface ISetupConfiguration
{
    void SetupConfiguration(IConfigurationBuilder builder);
}

public interface ISetupApplicationHost
{
    void SetupApplication(WebApplication app, CancellationToken token);
}