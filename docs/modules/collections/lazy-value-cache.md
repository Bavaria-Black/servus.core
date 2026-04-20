# LazyValueCache

Memory-cached lazy initialisation with per-entry expiration. Values are produced by a factory the first time they're requested and then returned from cache until they expire. Built on top of `Microsoft.Extensions.Caching.Memory.IMemoryCache`.

## Usage

```csharp
using Servus.Core.Collections;

var cache = new LazyValueCache<string, ExchangeRate>(
    defaultExpiration: TimeSpan.FromMinutes(5));

// First call: factory runs, result cached
var usd = cache.GetOrCreate("USD", () => FetchRate("USD"));

// Subsequent calls within 5 min: returned from cache (factory skipped)
var alsoUsd = cache.GetOrCreate("USD", () => FetchRate("USD"));
```

## Per-entry expiration

Override the default for a single entry:

```csharp
var rate = cache.GetOrCreate(
    "USD",
    () => FetchRate("USD"),
    expiration: TimeSpan.FromSeconds(30));
```

## Injecting your own `IMemoryCache`

Passing your own cache instance lets `LazyValueCache` share storage with other components in your app:

```csharp
var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
var cache = new LazyValueCache<string, ExchangeRate>(memoryCache, TimeSpan.FromMinutes(5));
```

## Peeking without creating

```csharp
if (cache.TryGetValue("USD", out var rate))
{
    // hit — rate is populated
}
else
{
    // miss — factory not invoked
}
```

## API

```csharp
public sealed class LazyValueCache<TKey, TValue> where TKey : notnull
{
    public TimeSpan DefaultExpiration { get; set; }

    public LazyValueCache(IMemoryCache? cache = null, TimeSpan? defaultExpiration = null);

    public TValue GetOrCreate(TKey key, Func<TValue> provider, TimeSpan? expiration = null);
    public bool TryGetValue(TKey key, out TValue? value);
}
```

The default expiration is **30 minutes** unless overridden in the constructor.
