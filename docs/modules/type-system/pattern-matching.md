# Pattern Matching

`WhenType<T>` — "run this block if the target is of type `T`". A chainable companion to [Conditional Invoke](./conditional-invoke) that reads naturally when you want a sequence of type checks.

## Usage

```csharp
using Servus.Core.Functional;

object value = GetSomething();

value.WhenType<string>(s  => Console.WriteLine($"string: {s}"));
value.WhenType<int>   (i  => Console.WriteLine($"int:    {i}"));
value.WhenType<Order> (o  => Process(o));
```

## Comparison with `InvokeIf`

Both check "is this object of type T?" and run a handler if so. Pick based on readability:

| | Use `WhenType<T>` for | Use `InvokeIf<T>` for |
|---|---|---|
| Intent | A value's concrete type | An interface/capability the target might support |
| Handler returns | `void` only | `void` or a value |
| Null handling | Target can be null, handler doesn't run | Target can be null, handler doesn't run |

## API

```csharp
public static class TypeExtensions
{
    public static void WhenType<T>(this object target, Action<T> handler);
}
```
