using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Servus.Core.Application.Startup;

namespace Servus.Core.Application.HealthChecks;

public abstract class HealthCheckSetupContainer : ApplicationSetupContainer<WebApplication>, IServiceSetupContainer
{
    protected virtual string UrlPattern { get; } = "healthz";
    
    protected sealed override void SetupApplication(WebApplication app)
    { 
        var builder =app.MapHealthChecks(UrlPattern);
        SetupEndpointConventions(builder);
    }

    public void SetupServices(IServiceCollection services, IConfiguration configuration)
    {
        var builder = services.AddHealthChecks();
        SetupHealthChecks(builder);
    }

    protected virtual void SetupEndpointConventions(IEndpointConventionBuilder builder)
    {
    }

    protected abstract void SetupHealthChecks(IHealthChecksBuilder builder);
}