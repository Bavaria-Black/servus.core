# TypeRegistry

A thin wrapper around `ConcurrentDictionary<Type, TValue>` with `GetOrAdd` semantics and a generic key API. Use it as the backing store for plug-in systems, per-type caches, and strategy dispatch.

## Usage

```csharp
using Servus.Core.Collections;

var registry = new TypeRegistry<Func<object, string>>();

registry.Add<Invoice>(o => $"INV-{((Invoice)o).Id}");
registry.Add<Order>  (o => $"ORD-{((Order)o).Id}");

// Retrieve by generic key
var invoiceFormatter = registry.Get<Invoice>();

// Or by Type
var orderFormatter = registry.Get(typeof(Order));
```

## `GetOrAdd` — lazy registration

Create-on-demand keeps the registry self-populating:

```csharp
var formatter = registry.GetOrAdd<Payment>(() => BuildFormatterFor<Payment>());
```

## API

```csharp
public class TypeRegistry<TValue>
{
    protected ConcurrentDictionary<Type, TValue> Dictionary { get; }

    public void Add<TKey>(TValue value);
    public void Add(Type key, TValue value);

    public TValue Get<TKey>();
    public TValue Get(Type key);

    public TValue GetOrAdd<TKey>(Func<TValue> factory);
    public TValue GetOrAdd(Type key, Func<TValue> factory);
}
```

`Get` throws when the key is missing. If you want a silent miss, use `GetOrAdd` with a fallback factory.

## Pattern: per-type handlers

`TypeRegistry<Action<object>>` is a minimal message-type dispatch table:

```csharp
var handlers = new TypeRegistry<Action<object>>();

handlers.Add<UserCreated>(e => OnUserCreated((UserCreated)e));
handlers.Add<OrderPlaced>(e => OnOrderPlaced((OrderPlaced)e));

void Dispatch(object evt) => handlers.Get(evt.GetType())(evt);
```
