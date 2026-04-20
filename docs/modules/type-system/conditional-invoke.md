# Conditional Invoke

`InvokeIf<T>` — "run this block only if the object implements `T`". It's a readable, null-safe replacement for the usual:

```csharp
if (obj is IDisposable d) d.Dispose();
```

pattern, and it has async and return-value variants.

## Key features

- **Type-safe** — only executes when the object is `T`
- **Null safety** — does nothing on a null target; no `NullReferenceException`
- **Async support** — `InvokeIfAsync` variants for `Task`-returning bodies
- **Return values** — function overloads return the result (or `default` when the type doesn't match)
- **Works with any reference type** — interface, abstract class, concrete class

## Usage

```csharp
using Servus.Core.Reflection;

object someObject = new FileStream("file.txt", FileMode.Open);

// Any reference type works — not just interfaces
someObject.InvokeIf<Stream>(s =>
{
    Console.WriteLine($"Stream length: {s.Length}");
    s.Seek(0, SeekOrigin.Begin);
});

// Only executes if someObject implements IDisposable
someObject.InvokeIf<IDisposable>(d => d.Dispose());
```

## Async version

```csharp
await client.InvokeIfAsync<IAsyncDisposable>(async d => await d.DisposeAsync());
```

## Return values

```csharp
string? name = entity.InvokeIf<INamed, string>(n => n.Name);
// null if 'entity' doesn't implement INamed, otherwise n.Name
```

```csharp
int? count = source.InvokeIf<ICollection, int>(c => c.Count);
```

## API

```csharp
public static class InterfaceInvokeExtensions
{
    public static void InvokeIf<TTarget>(this object entity, Action<TTarget> action)
        where TTarget : class;

    public static Task InvokeIfAsync<TTarget>(this object entity, Func<TTarget, Task> action)
        where TTarget : class;

    public static TResult? InvokeIf<TTarget, TResult>(this object entity, Func<TTarget, TResult> action)
        where TTarget : class;

    public static Task<TResult?> InvokeIfAsync<TTarget, TResult>(this object entity, Func<TTarget, Task<TResult>> action)
        where TTarget : class;
}
```

## When to use it

Any time you'd otherwise write `if (x is T t) t.DoThing()` more than a couple of times. Particularly handy for:

- **Clean-up** — `obj.InvokeIf<IDisposable>(d => d.Dispose())`
- **Optional interfaces** — "if this handler supports warm-up, call it"
- **Decorator patterns** — optional capabilities on otherwise-unrelated types
