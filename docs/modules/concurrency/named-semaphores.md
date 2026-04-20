# Named Semaphores

A reference-counted, name-keyed pool of `SemaphoreSlim` instances. `OpenOrCreate(name)` either creates a new semaphore or hands you back the existing one with the same name. When the last caller releases, the semaphore is cleaned up.

Use this when you need to serialize access by a **logical resource name** — e.g. per-user, per-file, per-tenant — without growing a dictionary of locks that never frees entries.

## Usage

```csharp
using Servus.Core.Concurrency;
using Servus.Core.Threading;

public async Task UpdateUserAsync(string userId, CancellationToken token)
{
    using var sem = NamedSemaphoreSlimStore.OpenOrCreate($"user:{userId}");
    using var _   = await sem.WaitScopedAsync(token);

    // Only one update per user runs at a time; updates to different
    // users run in parallel because they hit different semaphores.
    await DoTheUpdateAsync(userId, token);
}
```

Pair this with [Semaphore Scopes](./semaphore-scopes) to get `using`-based release.

## Initial and maximum count

The defaults are `initialCount: 1`, `maximumCount: 1` — a mutex. Override both to get a real counting semaphore:

```csharp
// At most 3 concurrent writers per tenant:
using var sem = NamedSemaphoreSlimStore.OpenOrCreate(
    name: $"tenant-writer:{tenantId}",
    defaultInitialCount: 3,
    defaultMaximumCount: 3);
```

Note: the default values only apply **on creation**. If the semaphore with that name already exists, it's returned as-is — the `defaultInitialCount`/`defaultMaximumCount` arguments are ignored. Keep the arguments consistent across call sites.

## Lifecycle

- Each `OpenOrCreate` increments the reference count.
- `Dispose` on the returned `NamedSemaphoreSlim` decrements it.
- When the count reaches zero, the semaphore is removed from the store and the underlying `SemaphoreSlim` is disposed.

Therefore — always call `Dispose` (easiest via `using`). Leaked handles mean the named slot stays alive forever.

## API

```csharp
public class NamedSemaphoreSlim : SemaphoreSlim
{
    public string Name { get; }
    // construction goes through NamedSemaphoreSlimStore.OpenOrCreate
}

public static class NamedSemaphoreSlimStore
{
    public static NamedSemaphoreSlim OpenOrCreate(
        string name,
        int defaultInitialCount = 1,
        int defaultMaximumCount = 1);
}
```
