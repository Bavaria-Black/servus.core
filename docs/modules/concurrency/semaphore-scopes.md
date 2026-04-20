# Semaphore Scopes

Two extension methods on `SemaphoreSlim` that give you `using`-friendly acquire/release. Saves you from the usual `try/finally` around `Wait` and `Release`.

## Usage

```csharp
using Servus.Core.Threading;

private readonly SemaphoreSlim _gate = new(1, 1);

public async Task DoThingAsync(CancellationToken token)
{
    using (await _gate.WaitScopedAsync(token))
    {
        // exclusive section
        await DoTheThingAsync(token);
    } // released automatically on scope exit (even on throw)
}

public void DoThing()
{
    using (_gate.WaitScoped())
    {
        // synchronous exclusive section
        DoTheThing();
    }
}
```

The returned `IDisposable` calls `Release` once when disposed. On exceptions inside the `using` block, release still happens via the `finally` semantics of `using`.

## API

```csharp
public static class SemaphoreSlimExtensions
{
    public static Task<IDisposable> WaitScopedAsync(
        this SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken = default);

    public static IDisposable WaitScoped(this SemaphoreSlim semaphoreSlim);
}
```
