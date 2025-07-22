using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Servus.Core.Application.Startup.Gates;

namespace Servus.Core.Application.Startup;

public class AppBuilder
{
    private readonly List<ISetupContainer> _appSetupContainer = [];
    private readonly List<IStartupGate> _gates = [];
    private readonly IHostApplicationBuilder _hostBuilder;
    private readonly Func<IHostApplicationBuilder, IHost> _hostFactory;
    
    internal Action<IServiceProvider> StartedAction = (_) => { };
    internal Action StoppedAction = () => { };
    internal Action StoppingAction = () => { };

    private AppBuilder(IHostApplicationBuilder hostBuilder, Func<IHostApplicationBuilder, IHost> hostFactory)
    {
        _hostBuilder = hostBuilder;
        _hostFactory = hostFactory;
    }

    public static AppBuilder Create<T>(T builder, Func<T, IHost> createHost) where T : IHostApplicationBuilder
    {
        return new AppBuilder(builder, b => createHost((T)b));
    }

    public static AppBuilder Create() => Create(WebApplication.CreateBuilder(), b => b.Build());

    public AppBuilder WithSetup<TContainer>() where TContainer : class, ISetupContainer, new() 
        => WithSetup(new TContainer());

    public AppBuilder WithSetup(ISetupContainer container)
    {
        _appSetupContainer.Add(container);
        return this;
    }

    public AppBuilder WithStartupGate(Func<Task<bool>> gate) => WithStartupGate(new ActionStartupGate(gate));
    
    public AppBuilder WithStartupGate<TGate>() where  TGate : class, IStartupGate, new() => WithStartupGate(new TGate());

    public AppBuilder WithStartupGate(IStartupGate gate)
    {
        _gates.Add(gate);
        return this;
    }

    public AppBuilder OnApplicationStarted(Action<IServiceProvider> started)
    {
        StartedAction = started;
        return this;
    }

    public AppBuilder OnApplicationStopping(Action stopping)
    {

        StoppingAction = stopping;
        return this;
    }

    public AppBuilder OnApplicationStopped(Action stopped) 
    {
        
        StoppedAction = stopped;
        return this;
    }

    public AppRunner Build() => new AppRunner(this);
    
    internal IReadOnlyCollection<TContainerType> GetContainer<TContainerType>() => _appSetupContainer.OfType<TContainerType>().ToList();
    internal IReadOnlyCollection<IStartupGate> GetGates() => _gates;
    
    internal IHostApplicationBuilder GetHostBuilder() => _hostBuilder;
    internal IHost BuildHost(IHostApplicationBuilder builder) => _hostFactory(builder);
}

public abstract class ApplicationSetupContainer : ISetupContainer
{
    internal void InjectApp(IApplicationBuilder app) => SetupApplication(app);
    
    protected abstract void SetupApplication(IApplicationBuilder app);
}