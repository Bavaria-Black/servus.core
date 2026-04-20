# Tracing

Distributed-tracing helpers built on top of `System.Diagnostics.ActivitySource`. Two things here:

1. **`IWithTracing`** — interface you put on messages/events so they carry `TraceId`/`SpanId` across boundaries (queues, gRPC, HTTP).
2. **`ActivitySourceRegistry`** — centralised `ActivitySource` management so you don't spawn one per class.

## `IWithTracing`

Stamp this interface onto any DTO that crosses a boundary:

```csharp
public class ProcessOrderMessage : IWithTracing
{
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }

    public int OrderId { get; set; }
    public decimal Amount { get; set; }

    public ActivityContext GetContext() => /* provided by the interface default logic */;
}
```

### Before sending — capture the current context

```csharp
var msg = new ProcessOrderMessage { OrderId = 42, Amount = 19.90m };

// Capture current Activity into TraceId/SpanId
((IWithTracing)msg).AddTracing();

await messageQueue.SendAsync(msg);
```

### Alternative context sources

```csharp
msg.AddTracing(otherTracing);              // copy from another IWithTracing
msg.AddTracing(traceId, spanId);           // set explicitly
```

## `ActivitySourceRegistry`

A static registry keyed by `Type`. Each type gets an `ActivitySource` whose name is automatically derived (snake_case of the type name) unless you override it.

```csharp
using Servus.Core.Diagnostics;

// Register once, early in startup
ActivitySourceRegistry.Add<OrderService>();
ActivitySourceRegistry.Add<PaymentService>("payments.core");
```

### Starting an activity from a message

`StartActivity<T>` uses the registered `ActivitySource` for `T` and the context from the incoming `IWithTracing` message:

```csharp
public class OrderService
{
    public async Task HandleOrderMessage(ProcessOrderMessage message)
    {
        using var activity = ActivitySourceRegistry.StartActivity<OrderService>(
            "process-order", message);

        await ProcessOrderAsync(message.OrderId);
    }
}
```

## `ActivitySourceNameAttribute`

Attach to a class so the registry uses a fixed name regardless of the type's own name:

```csharp
[ActivitySourceName("MyCompany.OrderProcessing")]
public class OrderService
{
    // ActivitySource name is resolved from the attribute, not derived.
}
```

This is the cleanest way to keep source names under your control.

## `ActivitySourceKeyAttribute`

Designate *another* class as the primary ActivitySource. Useful when a hierarchy of helper classes should roll up under one named source — you avoid having one ActivitySource per helper.

```csharp
[ActivitySourceKey(typeof(OrderService))]
public class SubOrderProcess
{
    public void DoWork()
    {
        // Resolves to OrderService's ActivitySource
        using var activity = ActivitySourceRegistry.StartActivity<SubOrderProcess>(
            "do-work", message);
    }
}

[ActivitySourceName("MyCompany.OrderProcessing")]
public class OrderService { }
```

## API

```csharp
public interface IWithTracing
{
    string? TraceId { get; set; }
    string? SpanId { get; set; }

    ActivityContext GetContext();
    void AddTracing();
    void AddTracing(IWithTracing tracing);
    void AddTracing(string? traceId, string? spanId);

    Activity? StartActivity(string name, ActivitySource source,
                            ActivityKind kind = ActivityKind.Consumer);
}

public static class ActivitySourceRegistry
{
    public static void Add<T>(string? name = null);
    public static void Add(Type key, string? name = null);

    public static Activity? StartActivity<T>(
        string activityName, IWithTracing trace,
        ActivityKind kind = ActivityKind.Consumer);

    public static Activity? StartActivity(
        Type key, string activityName, IWithTracing trace,
        ActivityKind kind = ActivityKind.Consumer);
}

[AttributeUsage(AttributeTargets.Class)]
public class ActivitySourceNameAttribute : Attribute
{
    public ActivitySourceNameAttribute(string name);
    public string ActivitySourceName { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class ActivitySourceKeyAttribute : Attribute
{
    public ActivitySourceKeyAttribute(Type sourceKey);
    public Type SourceKey { get; }
}
```
