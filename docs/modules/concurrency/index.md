# Concurrency

Primitives and helpers for dealing with asynchronous work, timers, locks, and background tasks — all designed to stay readable and not leak resources.

## Pages in this section

- [**Awaitable Condition**](./awaitable-condition) — wait on a custom condition with timeout / cancellation.
- [**Blocking Timer**](./blocking-timer) — periodic timer that never overlaps itself.
- [**Named Semaphores**](./named-semaphores) — reference-counted, name-keyed `SemaphoreSlim` pool.
- [**Semaphore Scopes**](./semaphore-scopes) — `using`-friendly `SemaphoreSlim` locking.
- [**Action Registry**](./action-registry) — DI-aware task registration + sequential/parallel execution.

## Namespace map

| Namespace | Types |
|---|---|
| `Servus.Core.Concurrency` | `NamedSemaphoreSlim`, `NamedSemaphoreSlimStore` |
| `Servus.Core.Threading` | `BlockingTimer`, `AwaitableCondition`, `SemaphoreSlimExtensions` |
| `Servus.Core.Threading.Tasks` | `ITaskMarker`, `IAsyncTask`, `IAsyncTask<T>`, `IActionRegistry<T>`, `IActionRegistryRunner<T>`, `ActionRegistry<T>`, `ActionRegistry<TIn,TOut>` |
