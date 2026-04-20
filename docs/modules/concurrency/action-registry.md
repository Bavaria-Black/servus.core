# Action Registry

A DI-aware registry for types or instances that you later run as a group — sequentially, in parallel, or as a streamed `IAsyncEnumerable<T>`. It's the execution machinery behind [Startup Gates](../application/startup-gates) and anywhere else you'd otherwise hand-roll a "run all these things" pattern.

## The contracts

```csharp
public interface ITaskMarker { }                      // marker

public interface IAsyncTask : ITaskMarker             // no return value
{
    ValueTask RunAsync(CancellationToken token);
}

public interface IAsyncTask<T> : ITaskMarker         // with return value
{
    ValueTask<T> RunAsync(CancellationToken token);
}

public interface IActionRegistry<T>                  // register
{
    void Register<TImplementation>() where TImplementation : T;
    void Register(T instance);
}

public interface IActionRegistryRunner<out T>        // execute
{
    void RunAll(
        IServiceProvider sp,
        Action<T, CancellationToken> executor,
        CancellationToken cancellationToken = default);

    ValueTask RunAsyncParallel(
        IServiceProvider sp,
        Func<T, CancellationToken, ValueTask> executor,
        CancellationToken cancellationToken = default);

    ValueTask RunAllAsync(
        IServiceProvider sp,
        Func<T, CancellationToken, ValueTask> executor,
        CancellationToken cancellationToken = default);
}
```

## Registering

Register either a type (resolved from the service provider on execution) or a concrete instance:

```csharp
using Servus.Core.Threading.Tasks;

var registry = new ActionRegistry<IHealthCheckTask>();

registry.Register<DatabaseHealthCheck>();         // resolved from DI
registry.Register<ExternalApiHealthCheck>();      // resolved from DI
registry.Register(new CustomStaticHealthCheck()); // use this instance directly
```

## Executing — sequential

```csharp
await registry.RunAllAsync(
    serviceProvider,
    async (task, ct) => await task.RunAsync(ct),
    cancellationToken);
```

## Executing — parallel

```csharp
await registry.RunAsyncParallel(
    serviceProvider,
    async (task, ct) => await task.RunAsync(ct),
    cancellationToken);
```

## Streamed results (`ActionRegistry<TIn, TOut>`)

When your tasks return a value (`IAsyncTask<T>`), the generic two-parameter registry gives you an `IAsyncEnumerable<T>` that yields each task's result as it completes:

```csharp
public class PriceCheckTask : IAsyncTask<decimal>
{
    public ValueTask<decimal> RunAsync(CancellationToken token) => /* … */;
}

var priceChecks = new ActionRegistry<PriceCheckTask, decimal>();
priceChecks.Register<UsdPriceCheck>();
priceChecks.Register<EurPriceCheck>();

await foreach (var price in priceChecks.RunAllAsync(serviceProvider, ct))
{
    Console.WriteLine($"Price: {price:C}");
}
```

## API

```csharp
public class ActionRegistry<T>
    : IActionRegistry<T>, IActionRegistryRunner<T>
{
    public void Register<TImplementation>() where TImplementation : T;
    public void Register(T instance);

    public void RunAll(IServiceProvider sp, Action<T, CancellationToken> executor,
                      CancellationToken cancellationToken = default);

    public ValueTask RunAsyncParallel(IServiceProvider sp,
                      Func<T, CancellationToken, ValueTask> executor,
                      CancellationToken cancellationToken = default);

    public ValueTask RunAllAsync(IServiceProvider sp,
                      Func<T, CancellationToken, ValueTask> executor,
                      CancellationToken cancellationToken = default);
}

public class ActionRegistry<TIn, TOut> : ActionRegistry<TIn>
    where TIn : IAsyncTask<TOut>
{
    public IAsyncEnumerable<TOut> RunAllAsync(
        IServiceProvider serviceProvider,
        CancellationToken token = default);
}
```

## When to use it

- **Startup orchestration** — run a set of checks/gates registered across multiple containers.
- **Background groups** — batch-run a configurable set of tasks with consistent cancellation.
- **Plug-in execution** — let modules register work into a shared registry resolved at runtime.

For smaller one-off cases a `Task.WhenAll` over a `List<Func<Task>>` is fine — reach for `ActionRegistry` when registration and execution happen in different parts of the system.
