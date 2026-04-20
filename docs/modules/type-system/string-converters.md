# String Converters

A small, pluggable system for converting strings into strongly-typed values. Five built-in converters cover the usual primitives, and you can plug in your own by implementing `IStringValueConverter`. A `StringConverterCollection` dispatches by target type.

## The contract

```csharp
public interface IStringValueConverter
{
    Type OutputType { get; }
    object? Convert(string value);
}
```

Each converter owns one output type. The collection routes `Convert<T>(string)` calls to the right one.

## Built-in converters

| Converter | `OutputType` | Notes |
|---|---|---|
| `BoolValueConverter` | `bool` | case-insensitive; extendable true/false value lists |
| `IntegerValueConverter` | `int` | `int.Parse` |
| `FloatValueConverter` | `float` | `float.Parse` |
| `DoubleValueConverter` | `double` | `double.Parse` |
| `StringValueConverter` | `string` | passthrough |

## Usage

```csharp
using Servus.Core.Conversion;
using Servus.Core.Collections; // InsertionBehavior

var converters = new StringConverterCollection();
converters.Register(new BoolValueConverter());
converters.Register(new IntegerValueConverter());
converters.Register(new DoubleValueConverter());
converters.Register(new StringValueConverter());

// Generic dispatch
int port      = (int)converters.Convert<int>("8080")!;
double ratio  = (double)converters.Convert<double>("0.75")!;
bool enabled  = (bool)converters.Convert<bool>("true")!;
string name   = (string)converters.Convert<string>("servus")!;

// Type-driven dispatch
object? value = converters.Convert(typeof(int), "42");
```

## Custom true/false values for `BoolValueConverter`

```csharp
var booly = new BoolValueConverter(
    additionalTrueValues:  ["yes", "on",  "1", "enabled"],
    additionalFalseValues: ["no",  "off", "0", "disabled"]);

converters.Register(booly, InsertionBehavior.OverwriteExisting);

bool flag = (bool)converters.Convert<bool>("enabled")!; // true
```

## Custom converter

Implement `IStringValueConverter` for your own types:

```csharp
public sealed class TimeSpanValueConverter : IStringValueConverter
{
    public Type OutputType => typeof(TimeSpan);

    public object? Convert(string value) =>
        TimeSpan.TryParse(value, out var ts)
            ? ts
            : throw new FormatException($"'{value}' is not a valid TimeSpan");
}

converters.Register(new TimeSpanValueConverter());
var duration = (TimeSpan)converters.Convert<TimeSpan>("00:05:30")!;
```

## Custom error handling

By default a parse failure propagates (typically `FormatException`). Register an exception handler to change that:

```csharp
converters.RegisterExceptionHandler(ex =>
{
    logger.LogWarning(ex, "Conversion failed — returning default");
    return null;
});

var maybe = converters.Convert<int>("not-a-number"); // returns null instead of throwing
```

## Insertion behaviour

`Register` takes an optional `InsertionBehavior` to control what happens when a converter for the same output type is already registered:

- `None` — silently ignore the new registration (default)
- `OverwriteExisting` — replace the existing converter
- `ThrowOnExisting` — throw if one is already registered

See [`InsertionBehavior`](/modules/collections/extensions#insertionbehavior).

## API

```csharp
public class StringConverterCollection
{
    public void RegisterExceptionHandler(Func<Exception, object?> handler);
    public void Register(IStringValueConverter converter, InsertionBehavior behavior = InsertionBehavior.None);

    public object? Convert<T>(string value);
    public object? Convert(Type targetType, string value);
}
```
