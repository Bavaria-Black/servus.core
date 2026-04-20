# Type Checking

Two tiny extension methods that wrap `is`/`as` and `Convert.ChangeType` into APIs that don't throw when you don't want them to.

## `TryConvert<T>`

Returns `true` and the converted value when the item is (or can be coerced to) `T`, `false` otherwise. No exceptions.

```csharp
using Servus.Core.Reflection;

object raw = "42";

if (raw.TryConvert<int>(out var number))
{
    Console.WriteLine(number); // 42
}
```

`TryConvert` handles both direct casts and `IConvertible` conversions, so you can go `string → int` without thinking about the underlying mechanism.

## `Convert<T>`

Throws if the conversion isn't possible. Use it when the failure is a bug and you want the stack trace:

```csharp
int port = settings.Port.Convert<int>();
```

## `Convert<T>` with a function

Provide your own converter function — useful when the conversion isn't a standard cast:

```csharp
var color = hexString.Convert<string, Color>(s => ColorTranslator.FromHtml(s));
```

## API

```csharp
public static class TypeCheckExtensions
{
    public static bool TryConvert<TTarget>(this object? item, out TTarget? value);
    public static TTarget Convert<TTarget>(this object? item);
    public static TTarget? Convert<TTarget>(this object? item, Func<object, TTarget> converter);
    public static TTarget? Convert<TInput, TTarget>(this object? item, Func<TInput, TTarget> converter);
}
```
