# Type System

Helpers for converting, checking, and conditionally invoking based on runtime types — the kind of thing you write once per project and then copy into every new one.

## Pages in this section

- [**String Converters**](./string-converters) — pluggable `string → T` conversion with a registry.
- [**Type Checking**](./type-checking) — safe `TryConvert`/`Convert` helpers that don't throw when you don't want them to.
- [**Conditional Invoke**](./conditional-invoke) — "do this if the object is `T`" without manual `is` / `as` dances.
- [**Pattern Matching**](./pattern-matching) — `WhenType<T>` for chained type-based handlers.

## Namespace map

| Namespace | Types |
|---|---|
| `Servus.Core.Conversion` | `IStringValueConverter`, `BoolValueConverter`, `IntegerValueConverter`, `FloatValueConverter`, `DoubleValueConverter`, `StringValueConverter`, `StringConverterCollection` |
| `Servus.Core.Reflection` | `InterfaceInvokeExtensions`, `TypeCheckExtensions` |
| `Servus.Core.Functional` | `TypeExtensions` (`WhenType`) |
