using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Servus.Core.Collections;

public sealed class LazyValueCache<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary = [];
    
    public TValue Get(TKey type, Func<TValue> provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        
        if(_dictionary.TryGetValue(type, out var value)) return value;
        
        value = provider();
        _dictionary[type] = value;
        
        return value;
    }

    public bool TryPeek(TKey type, [NotNullWhen(true)] out TValue? value)
    {
        return _dictionary.TryGetValue(type, out value);
    }
}