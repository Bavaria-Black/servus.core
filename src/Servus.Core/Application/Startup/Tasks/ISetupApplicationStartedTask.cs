using System;

namespace Servus.Core.Application.Startup.Tasks;

public interface ISetupApplicationStartedTask
{
    void OnApplicationStarted(IServiceProvider serviceProvider);
}