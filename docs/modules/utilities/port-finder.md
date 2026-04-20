# Port Finder

`PortFinder.FindFreeLocalPort()` returns an unused TCP port on the local machine, or `0` if none could be found. Useful for spinning up test servers, child processes, or dev-time integration harnesses without hard-coding ports.

## Usage

```csharp
using Servus.Core.Network;

int port = PortFinder.FindFreeLocalPort();

if (port == 0)
    throw new InvalidOperationException("No free port available");

var server = WebApplication.CreateBuilder().Build();
server.Urls.Add($"http://localhost:{port}");
await server.StartAsync();
```

The method binds a socket, reads the assigned port, and releases immediately — there's an inherent race between the call and the next binder. In practice it's fine for tests and dev tooling; don't use it in a tight retry loop in production.

## API

```csharp
public class PortFinder
{
    public static int FindFreeLocalPort();
}
```
