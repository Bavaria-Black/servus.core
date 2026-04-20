# BlockingTimer

A timer that guarantees **non-overlapping** execution — if the previous tick is still running when the next one would fire, the new tick is skipped. Use this anywhere you'd otherwise reach for `System.Threading.Timer` and then remember halfway through that overlapping invocations will eat your lunch.

## Usage

```csharp
using Servus.Core.Threading;

using var cts = new CancellationTokenSource();

using var timer = new BlockingTimer(
    timerAction:            () => ProcessBatch(),
    intervalInMilliseconds: 5000,
    cancellationToken:      cts.Token);

// Timer starts immediately on construction.
// Dispose or Stop() waits for the currently-running action to finish.
```

## Semantics

- The timer **starts on construction**. No `Start()` needed.
- If the action is still running when the interval elapses, the next tick is **skipped** (not queued).
- `Stop()` cancels the internal loop and blocks until the current action completes.
- `Dispose()` calls `Stop()` and releases resources.

## API

```csharp
public sealed class BlockingTimer : IDisposable
{
    public BlockingTimer(
        Action timerAction,
        double intervalInMilliseconds,
        CancellationToken cancellationToken = default);

    public void Stop();
    public void Dispose();
}
```

## When to use it

- **Periodic polling** where a slow tick must not overlap the next scheduled tick.
- **Background batches** that reach external systems and have variable latency.
- **Long-running tasks** that need a floor on spacing but not a strict cadence.

## When NOT to use it

- Sub-second cadence with sub-millisecond drift requirements — use a dedicated scheduler.
- Work that *must* run at every interval even if the previous tick is still alive (use `System.Threading.Timer` and deal with concurrency yourself).
