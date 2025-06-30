using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Servus.Core.Application.Startup.Tasks;
using Servus.Core.Reflection;

namespace Servus.Core.Application;

public static class AppStarter
{
    public static async Task RunAsync<T>(CancellationToken token = default)
        where T : AppConfigurationBase, new()
    {
        var instance = new T();
        await RunAsync(instance, token).ConfigureAwait(false);
    }

    public static async Task RunAsync(AppConfigurationBase configuration, CancellationToken token = default)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        var builder = WebApplication.CreateBuilder(Environment.GetCommandLineArgs());

        await RunAsync(builder, configuration, cts.Token).ConfigureAwait(false);
    }

    public static async Task RunAsync(WebApplicationBuilder builder, AppConfigurationBase configuration,
        CancellationToken token = default)
        => await InternalStartAsync(builder, configuration, app => app.RunAsync(), token);
    
    public static async Task StartAsync<T>(CancellationToken token)
        where T : AppConfigurationBase, new()
    {
        var instance = new T();
        await StartAsync(instance, token).ConfigureAwait(false);
    }

    public static async Task StartAsync(AppConfigurationBase configuration, CancellationToken token)
    {
        var builder = WebApplication.CreateBuilder(Environment.GetCommandLineArgs());
        await StartAsync(builder, configuration, token).ConfigureAwait(false);
    }

    public static async Task StartAsync(WebApplicationBuilder builder, AppConfigurationBase configuration,
        CancellationToken token)
        => await InternalStartAsync(builder, configuration, app => app.StartAsync(token), token);

    private static async Task InternalStartAsync(WebApplicationBuilder builder, AppConfigurationBase configuration,
        Func<WebApplication, Task> startupMethod, CancellationToken token)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        try
        {
            var app = await CoreSetupAsync(builder, configuration, cts);
            await startupMethod(app).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
    }

    private static Task<WebApplication> CoreSetupAsync(WebApplicationBuilder builder,
        AppConfigurationBase configuration,
        CancellationTokenSource cts)
    {
        configuration.SetupServices(builder.Services, builder.Configuration);

        var app = builder.Build();
        configuration.InvokeIf<ISetupApplicationHost>(d => d.SetupApplication(app, cts.Token));

        SetupApplicationLifetime(configuration, app);

        return Task.FromResult(app);
    }

    private static void SetupApplicationLifetime(AppConfigurationBase configuration, WebApplication app)
    {
        configuration.InvokeIf<ISetupApplicationStartedTask>(t =>
            app.Lifetime.ApplicationStarted.Register(() => t.OnApplicationStarted(app.Services)));
        configuration.InvokeIf<ISetupApplicationShutdownTask>(t =>
            app.Lifetime.ApplicationStopping.Register(() => t.OnApplicationShuttingDown(app.Services)));
        configuration.InvokeIf<ISetupApplicationStoppedTask>(t =>
            app.Lifetime.ApplicationStopped.Register(t.OnApplicationStopped));
    }
}