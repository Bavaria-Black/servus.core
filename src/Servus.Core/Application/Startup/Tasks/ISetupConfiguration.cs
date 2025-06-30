using System;
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

public interface ISetupApplicationStartedTask
{
    void OnApplicationStarted(IServiceProvider serviceProvider);
}

public interface ISetupApplicationShutdownTask
{
    void OnApplicationShuttingDown(IServiceProvider serviceProvider);
}

public interface ISetupApplicationStoppedTask
{
    void OnApplicationStopped();
}