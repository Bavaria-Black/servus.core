using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Servus.Core.Reflection;

namespace Servus.Core.Application.Startup;

public partial class AppRunner
{
    private void SetupApplication(IApplicationBuilder app)
    {
        var webContainer = _appContainer.OfType<ApplicationSetupContainer<WebApplication>>().ToArray();
        var normalContainer = _appContainer.Except(webContainer);
        
        app.InvokeIf<WebApplication>(a => SetupWebApplication(a, webContainer));
        normalContainer.ForEach(c => InvokeApplicationSetupContainer(app, c));
    }

    private void InvokeApplicationSetupContainer(IApplicationBuilder app,  ApplicationSetupContainer container)
    {
        container.InjectApp(app);
    }

    private void SetupWebApplication(WebApplication app, IEnumerable<ApplicationSetupContainer<WebApplication>> webContainer)
    {
        webContainer.ForEach(c => c.InjectApp(app));
        
        SetupApplicationLifetime(app);
    }

    private void SetupApplicationLifetime(WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() => _baseBuilder.StartedAction(app.Services));
        app.Lifetime.ApplicationStopping.Register(() => _baseBuilder.StoppingAction());
        app.Lifetime.ApplicationStopped.Register(() => _baseBuilder.StoppedAction());
    }
}