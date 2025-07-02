using System.Threading;
using Microsoft.AspNetCore.Builder;

namespace Servus.Core.Application.Startup;

public interface ISetupApplicationHost
{
    void SetupApplication(WebApplication app, CancellationToken token);
}