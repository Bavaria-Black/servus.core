# Setup Containers

Setup containers split your DI and middleware wiring into small, focused classes. Each container implements one or more lifecycle interfaces, and [`AppBuilder`](./app-builder) calls them in the right order. The point is **testability and reuse** — each container is a plain class you can also instantiate in unit tests or reuse across projects.

## The contracts

All container interfaces extend the marker interface `ISetupContainer`. Implement the ones you need; `AppBuilder` routes calls to the matching methods.

| Interface | Called during | Use for |
|---|---|---|
| `IConfigurationSetupContainer` | Configuration build | Adding custom configuration sources (Vault, remote providers) |
| `IServiceSetupContainer` | DI registration | `services.AddX()` / `services.Configure<T>()` |
| `ILoggingSetupContainer` | Logging build | Adding log providers, filters |
| `IHostBuilderSetupContainer` | Host builder phase | Host-level hooks (rarely needed) |
| `IHostApplicationBuilderSetupContainer` | Host app builder | Newer hosting model hooks |
| `ApplicationSetupContainer` / `ApplicationSetupContainer<THost>` | App pipeline build | Middleware, endpoint mapping (`app.Use…`, `app.Map…`) |

```csharp
public interface ISetupContainer { }

public interface IConfigurationSetupContainer : ISetupContainer
{
    void SetupConfiguration(IConfigurationManager builder);
}

public interface IServiceSetupContainer : ISetupContainer
{
    void SetupServices(IServiceCollection services, IConfiguration configuration);
}

public interface ILoggingSetupContainer : ISetupContainer
{
    void SetupLogging(ILoggingBuilder builder);
}
```

## A typical container

A container can implement multiple interfaces. Here's a gRPC container that registers services and wires up the pipeline:

```csharp
public class GrpcSetupContainer
    : ApplicationSetupContainer<WebApplication>, IServiceSetupContainer
{
    public void SetupServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    protected override void SetupApplication(WebApplication app)
    {
        app.UseGrpcWeb();
        app.UseSwagger().UseSwaggerUI();
        app.MapGrpcService<GreeterService>().EnableGrpcWeb();
    }
}
```

## `ApplicationSetupContainer`

Abstract base for containers that need to configure the app pipeline. Two variants:

```csharp
public abstract class ApplicationSetupContainer : ISetupContainer
{
    protected abstract void SetupApplication(IApplicationBuilder app);
}

public abstract class ApplicationSetupContainer<THost> : ApplicationSetupContainer
    where THost : IApplicationBuilder
{
    protected abstract void SetupApplication(THost app);
}
```

Use the generic variant when you need access to `WebApplication`-specific methods (`MapGrpcService`, endpoint routing). Use the non-generic base when `IApplicationBuilder` is enough.

## Composing containers

Order your `WithSetup` calls by what depends on what. A pattern that works well:

```csharp
var app = AppBuilder.Create()
    .WithSetup<ConfigSetup>()        // first: extra config providers
    .WithSetup<LoggingSetup>()       // then: logging
    .WithSetup<DatabaseSetup>()      // DI: infra
    .WithSetup<DomainSetup>()        // DI: business logic
    .WithSetup<ApiSetup>()           // DI + middleware: HTTP surface
    .Build();
```

## Reusing containers in tests

Because containers are plain classes, you can instantiate them in unit tests without touching `AppBuilder`:

```csharp
var services = new ServiceCollection();
var configuration = new ConfigurationBuilder().Build();

new DatabaseSetup().SetupServices(services, configuration);

var provider = services.BuildServiceProvider();
var repo = provider.GetRequiredService<IUserRepository>();
```
