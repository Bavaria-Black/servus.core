# Health Checks

`HealthCheckSetupContainer` is a base class that wires ASP.NET Core's health-check infrastructure and exposes an endpoint (default `/healthz`) with minimal ceremony. Inherit, register your checks, and attach to your app.

## Basic usage

```csharp
public class HealthSetup : HealthCheckSetupContainer
{
    protected override void SetupHealthChecks(IHealthChecksBuilder builder)
    {
        builder
            .AddSqlServer(connectionString: "…")
            .AddRedis("localhost:6379")
            .AddCheck<CustomBusinessHealthCheck>("business");
    }
}

var app = AppBuilder.Create()
    .WithSetup<HealthSetup>()
    .Build();
```

This gives you a live `GET /healthz` that reports aggregated status from every registered check.

## Overriding the URL pattern

The default is `"healthz"`. Override to change it:

```csharp
public class HealthSetup : HealthCheckSetupContainer
{
    protected override string UrlPattern => "internal/health";

    protected override void SetupHealthChecks(IHealthChecksBuilder builder)
    {
        builder.AddCheck<MyCheck>("my-check");
    }
}
```

## Locking down the endpoint

Override `SetupEndpointConventions` to apply auth, CORS, or any other endpoint metadata:

```csharp
public class HealthSetup : HealthCheckSetupContainer
{
    protected override void SetupHealthChecks(IHealthChecksBuilder builder)
        => builder.AddCheck<MyCheck>("my-check");

    protected override void SetupEndpointConventions(IEndpointConventionBuilder builder)
    {
        builder
            .RequireAuthorization("InternalOnly")
            .WithMetadata(new AllowAnonymousAttribute());
    }
}
```

## Contract

```csharp
public abstract class HealthCheckSetupContainer
    : ApplicationSetupContainer<WebApplication>, IServiceSetupContainer
{
    protected virtual string UrlPattern { get; }       // default: "healthz"
    protected abstract void SetupHealthChecks(IHealthChecksBuilder builder);
    protected virtual void SetupEndpointConventions(IEndpointConventionBuilder builder);
}
```

`HealthCheckSetupContainer` implements both `IServiceSetupContainer` (for `AddHealthChecks()`) and `ApplicationSetupContainer<WebApplication>` (for `MapHealthChecks(UrlPattern)`) — you don't wire either manually.
