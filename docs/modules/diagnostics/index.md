# Diagnostics

Distributed tracing helpers built on top of `System.Diagnostics.ActivitySource`. The `IWithTracing` interface lets you carry tracing context on messages/events, and the `ActivitySourceRegistry` centralises `ActivitySource` management so you don't end up with one per class.

## Pages in this section

- [**Tracing**](./tracing) — `IWithTracing`, `ActivitySourceRegistry`, `ActivitySourceNameAttribute`, `ActivitySourceKeyAttribute`.

## Namespace map

| Namespace | Types |
|---|---|
| `Servus.Core.Diagnostics` | `IWithTracing`, `ActivitySourceRegistry`, `ActivitySourceNameAttribute`, `ActivitySourceKeyAttribute` |
