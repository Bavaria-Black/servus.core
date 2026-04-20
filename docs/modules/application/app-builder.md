# AppBuilder

`AppBuilder` is a fluent entry point that replaces the usual `Program.cs` boilerplate. It composes a host (by default a `WebApplication`), lets you attach modular setup containers, registers startup gates, and gives you lifecycle hooks — all before returning an `AppRunner` you can `RunAsync`.

## Quick start

```csharp
using Servus.Core.Application.Startup;

var app = AppBuilder
    .Create()
    .WithSetup<GrpcSetupContainer>()
    .WithSetup<LoggingSetupContainer>()
    .Build();

await app.RunAsync();
```

`AppBuilder.Create()` uses `WebApplication.CreateBuilder()` by default. If you need a different host type (e.g. `HostApplicationBuilder` for a console worker), pass your own:

```csharp
var app = AppBuilder
    .Create(Host.CreateApplicationBuilder(args), b => b.Build())
    .WithSetup<MyWorkerSetup>()
    .Build();
```

## Attaching setup containers

`WithSetup<T>()` and `WithSetup(instance)` attach one or more [Setup Containers](./setup-containers). Each container implements one or more of the lifecycle interfaces (`IServiceSetupContainer`, `IConfigurationSetupContainer`, `ILoggingSetupContainer`, `ApplicationSetupContainer`) and `AppBuilder` calls them in the right order.

```csharp
var app = AppBuilder
    .Create()
    .WithSetup<ConfigSetup>()       // IConfigurationSetupContainer
    .WithSetup<ServicesSetup>()     // IServiceSetupContainer
    .WithSetup<LoggingSetup>()      // ILoggingSetupContainer
    .WithSetup<MiddlewareSetup>()   // ApplicationSetupContainer<WebApplication>
    .Build();
```

Containers are the split-and-compose unit of a Servus app. Keep each container focused on one concern — you reuse them across projects and in unit tests.

## Startup gates

Block the app from starting until something outside is ready (a database, an upstream API, a license server, a file to appear). See [Startup Gates](./startup-gates) for the full flow.

```csharp
var app = AppBuilder
    .Create()
    .WithSetup<ServicesSetup>()
    .WithStartupGate(async () => await db.CanConnectAsync())
    .WithStartupGate<LicenseServerGate>()
    .Build();

await app.RunAsync();
```

## Lifecycle hooks

Run code at the three standard hosting transitions:

```csharp
var app = AppBuilder
    .Create()
    .WithSetup<ServicesSetup>()
    .OnApplicationStarted(sp => sp.GetRequiredService<IMetrics>().MarkStart())
    .OnApplicationStopping(() => Log.Information("Going down…"))
    .OnApplicationStopped(() => Log.Information("Bye."))
    .Build();
```

## Full API

```csharp
public class AppBuilder
{
    public static AppBuilder Create<T>(T builder, Func<T, IHost> createHost) where T : IHostApplicationBuilder;
    public static AppBuilder Create();

    public AppBuilder WithSetup<TContainer>() where TContainer : class, ISetupContainer, new();
    public AppBuilder WithSetup(ISetupContainer container);

    public AppBuilder WithStartupGate(Func<Task<bool>> gate);
    public AppBuilder WithStartupGate<TGate>() where TGate : class, IStartupGate, new();
    public AppBuilder WithStartupGate(IStartupGate gate);

    public AppBuilder OnApplicationStarted(Action<IServiceProvider> started);
    public AppBuilder OnApplicationStopping(Action stopping);
    public AppBuilder OnApplicationStopped(Action stopped);

    public AppRunner Build();
}

public partial class AppRunner
{
    public Task RunAsync(CancellationToken token = default);
    public Task StartAsync(CancellationToken token);
}
```
