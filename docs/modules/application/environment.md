# Servus Environment

A small helper for reading `SERVUS_*` prefixed environment variables, plus a handful of ASCII brand constants for startup banners.

## Reading environment variables

Every call prefixes `SERVUS_` automatically — so you pass the local name, not the fully qualified one.

```csharp
using Servus.Core.Application;

// Reads the env var "SERVUS_REGION"
var region = ServusApplication.GetEnvironmentVariable("REGION");

// Checks whether "SERVUS_FEATURE_X" equals "on" (case-insensitive)
if (ServusApplication.IsEnvironmentVariableSetTo("FEATURE_X", "on"))
{
    EnableExperimentalPath();
}
```

Both methods are `static` and can be called before DI is available, which is why they exist — use them during very early startup when you can't resolve services yet.

```csharp
public static class ServusApplication
{
    public static string? GetEnvironmentVariable(string name);
    public static bool IsEnvironmentVariableSetTo(string name, string value);
}
```

## Brand constants

Three ASCII logos in decreasing size. Useful for startup banners and `--version` output.

```csharp
using Servus.Core.Application;

Console.WriteLine(Servus.Logo);       // big
Console.WriteLine(Servus.LogoSmall);  // medium
Console.WriteLine(Servus.LogoTiny);   // minimal
```

```csharp
public static class Servus
{
    public static string Logo { get; }
    public static string LogoSmall { get; }
    public static string LogoTiny { get; }
}
```

## Service resolution helpers

Extension methods for resolving services, including types that aren't registered in DI but whose constructor dependencies are:

```csharp
using Servus.Core.Application;

// Resolve a registered service
var repo = serviceProvider.ResolveExternal<IUserRepository>();

// Or by Type — the dependency *is* registered, but the target type itself
// does not need to be registered. Constructor arguments are filled from DI.
var worker = serviceProvider.ResolveExternal(typeof(BackgroundWorker));
```

```csharp
public static class ServiceProviderExtensions
{
    public static T ResolveExternal<T>(this IServiceProvider serviceProvider);
    public static object ResolveExternal(this IServiceProvider serviceProvider, Type type);
}
```
