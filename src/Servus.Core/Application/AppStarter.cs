using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Servus.Core.Application.Startup;
using Servus.Core.Application.Startup.Tasks;
using Servus.Core.Reflection;


namespace Servus.Core.Application;

/// <summary>
/// Provides static methods for starting and running web applications with custom configuration.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class AppStarter
{
    /// <summary>
    /// Creates and runs a web application using the specified configuration type.
    /// </summary>
    /// <typeparam name="T">The configuration type that inherits from AppConfigurationBase and has a parameterless constructor.</typeparam>
    /// <param name="token">The cancellation token to cancel the operation. Default is default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous run operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    public static async Task RunAsync<T>(CancellationToken token = default)
        where T : AppConfigurationBase, new()
    {
        var instance = new T();
        await RunAsync(instance, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Runs a web application using the specified configuration instance.
    /// </summary>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="token">The cancellation token to cancel the operation. Default is default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous run operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    public static async Task RunAsync(AppConfigurationBase configuration, CancellationToken token = default)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        var builder = WebApplication.CreateBuilder(Environment.GetCommandLineArgs());

        await RunAsync(builder, configuration, cts.Token).ConfigureAwait(false);
    }

    /// <summary>
    /// Runs a web application using the specified builder and configuration.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="token">The cancellation token to cancel the operation. Default is default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous run operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder or configuration is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    public static async Task RunAsync(WebApplicationBuilder builder, AppConfigurationBase configuration,
        CancellationToken token = default)
        => await InternalStartAsync(builder, configuration, app => app.RunAsync(), token);
    
    /// <summary>
    /// Creates and starts a web application using the specified configuration type without blocking.
    /// </summary>
    /// <typeparam name="T">The configuration type that inherits from AppConfigurationBase and has a parameterless constructor.</typeparam>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    public static async Task StartAsync<T>(CancellationToken token)
        where T : AppConfigurationBase, new()
    {
        var instance = new T();
        await StartAsync(instance, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Starts a web application using the specified configuration instance without blocking.
    /// </summary>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    public static async Task StartAsync(AppConfigurationBase configuration, CancellationToken token)
    {
        var builder = WebApplication.CreateBuilder(Environment.GetCommandLineArgs());
        await StartAsync(builder, configuration, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Starts a web application using the specified builder and configuration without blocking.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder or configuration is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
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