# CircularQueue

A generic FIFO queue with a fixed capacity. Once full, each new item silently drops the oldest. Perfect for log tails, telemetry buffers, or any "last N things" scenario.

## Usage

```csharp
using Servus.Core.Collections;

var buffer = new CircularQueue<string>(3);

buffer.Enqueue("A");
buffer.Enqueue("B");
buffer.Enqueue("C");
// Queue: [A, B, C]

buffer.Enqueue("D");
// Queue: [B, C, D] — "A" was automatically removed

if (buffer.TryDequeue(out var item))
{
    Console.WriteLine(item); // "B"
}
```

## Reading without consuming

`Items` gives you an `IEnumerable<T>` snapshot without mutating the queue:

```csharp
foreach (var entry in buffer.Items)
{
    Log.Information("Tail entry: {Entry}", entry);
}
```

## API

```csharp
public class CircularQueue<T>
{
    public CircularQueue(int capacity);

    public int Count { get; }
    public int Capacity { get; }
    public IEnumerable<T> Items { get; }

    public void Enqueue(T item);
    public bool TryDequeue(out T item);
}
```

## When to use it

- **Log tails** — keep the last *N* log entries in memory for display on a diagnostics page.
- **Rolling metrics** — last *N* latencies or error messages for a running average.
- **Undo stacks** — limited-depth history where old entries should simply fall off.
