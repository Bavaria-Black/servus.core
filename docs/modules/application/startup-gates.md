# Startup Gates

A **startup gate** is a check that must return `true` before your application is allowed to finish starting up. They're the right tool when you want to block the app from serving traffic until an external dependency is actually reachable, a license has been validated, or a prerequisite file exists.

Gates run *after* services are built but *before* `IHostedService`s start. If a gate returns `false`, [`AppBuilder`](./app-builder) keeps calling it with back-off until it clears or the host is cancelled.

## Ad-hoc gate from a lambda

```csharp
var app = AppBuilder.Create()
    .WithSetup<ServicesSetup>()
    .WithStartupGate(async () =>
    {
        using var scope = /* resolve your DbContext */;
        return await scope.Database.CanConnectAsync();
    })
    .Build();

await app.RunAsync();
```

## Typed gates

For reusable gates, implement `IStartupGate` and register by type. The gate is resolved from DI, so it can depend on other services.

```csharp
public class LicenseServerGate : IStartupGate
{
    private readonly ILicenseClient _client;

    public LicenseServerGate(ILicenseClient client) => _client = client;

    public async Task<bool> CheckAsync(CancellationToken token = default)
        => await _client.IsReachableAsync(token);
}

var app = AppBuilder.Create()
    .WithSetup<ServicesSetup>()
    .WithStartupGate<LicenseServerGate>()
    .Build();
```

## Registering gates from a container

If your gates belong to a setup container, implement `ISetupStartupGates` on the container and register gates with the provided registry:

```csharp
public class ExternalDepsSetup
    : ISetupContainer, ISetupStartupGates
{
    public void OnRegisterStartupGates(IActionRegistry<IStartupGate> registry)
    {
        registry.Register<DatabaseGate>();
        registry.Register<LicenseServerGate>();
        registry.Register(new ApiReachableGate("https://api.example.com/health"));
    }
}
```

This keeps the gate registration close to the container that owns those dependencies.

## Contracts

```csharp
public interface IStartupGate
{
    Task<bool> CheckAsync(CancellationToken token = default);
}

public interface ISetupStartupGates
{
    void OnRegisterStartupGates(IActionRegistry<IStartupGate> registry);
}
```

## When to use a gate vs a health check

- **Gate** — "don't start serving until this is true". Blocks startup.
- **Health check** — "while running, report whether this is true". Does not block startup; used by load balancers / orchestrators.

Use both for truly critical dependencies: a gate to avoid serving while unready, and a health check so your orchestrator can react if the dependency dies later.
