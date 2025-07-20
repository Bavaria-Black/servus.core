using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Servus.Core.Application.Startup.Gates;
using Servus.Core.Reflection;

namespace Servus.Core.Application.Startup;

/// <summary>
/// Provides static methods for starting and running web applications with custom configuration.
/// </summary>
public partial class AppRunner
{
    private readonly IReadOnlyCollection<IServiceSetupContainer> _serviceContainer;
    private readonly IReadOnlyCollection<ApplicationSetupContainer> _appContainer;
    private readonly IReadOnlyCollection<IConfigurationSetupContainer> _configContainer;
    private readonly IReadOnlyCollection<ILoggingSetupContainer> _loggingContainer;
    
    private readonly IReadOnlyCollection<IStartupGate> _startupGates;
    private readonly IHostApplicationBuilder _builder;
    private readonly AppBuilder _baseBuilder;

    internal AppRunner(AppBuilder appBuilder)
    {
        _baseBuilder = appBuilder;
        
        _builder = appBuilder.GetHostBuilder();
        
        _serviceContainer = appBuilder.GetContainer<IServiceSetupContainer>();
        _appContainer = appBuilder.GetContainer<ApplicationSetupContainer>();
        _configContainer = appBuilder.GetContainer<IConfigurationSetupContainer>();
        _loggingContainer = appBuilder.GetContainer<ILoggingSetupContainer>();
        _startupGates = appBuilder.GetGates();
    }
    
    /// <summary>
    /// Runs a web application using the specified builder and configuration.
    /// </summary>
    /// <param name="token">The cancellation token to cancel the operation. Default is default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous run operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder or configuration is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    public async Task RunAsync(CancellationToken token = default)
        => await InternalStartAsync(app => app.RunAsync(), token);
    
    /// <summary>
    /// Creates and starts a web application using the specified configuration type without blocking.
    /// </summary>
    /// <typeparam name="T">The configuration type that inherits from AppConfigurationBase and has a parameterless constructor.</typeparam>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    public async Task StartAsync(CancellationToken token)
        => await InternalStartAsync(app => app.StartAsync(token), token);

    private async Task InternalStartAsync(Func<IHost, Task> startupMethod, CancellationToken token)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        try
        {
            var app = CoreSetupAsync();
            await RunStartupGatesAsync(app.Services, cts.Token);
            await startupMethod(app).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await cts.CancelAsync();
            System.Console.WriteLine(e);
            throw;
        }
    }

    private IHost CoreSetupAsync()
    {
        _loggingContainer.ForEach(c => c.SetupLogging(_builder.Logging));
        _configContainer.ForEach(c => c.SetupConfiguration(_builder.Configuration));
        _serviceContainer.ForEach(c => c.SetupServices(_builder.Services, _builder.Configuration));
        
        var app = _baseBuilder.BuildHost(_builder);
        app.InvokeIf<IApplicationBuilder>(SetupApplication);
        
        return app;
    }
}