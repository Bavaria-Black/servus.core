using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;

namespace Servus.Core.Collections;

public sealed class LazyValueCache<TKey, TValue> where TKey : notnull
{
    private readonly IMemoryCache _cache;
    
    public TimeSpan DefaultExpiration { get; set; }
    
    public LazyValueCache(IMemoryCache? cache = null, TimeSpan? defaultExpiration = null)
    {
        _cache = cache ?? new MemoryCache(new MemoryCacheOptions());
        DefaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(30);
    }

    public TValue GetOrCreate(TKey type, Func<TValue> provider, TimeSpan? expiration = null)
    {
        ArgumentNullException.ThrowIfNull(provider);
        return _cache.GetOrCreate<TValue>(type,f =>
        {
            f.AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration;
            return provider();
        })!;
    }

    public bool TryPeek(TKey type, [NotNullWhen(true)] out TValue? value)
    {
        return _cache.TryGetValue(type, out value);
    }
}