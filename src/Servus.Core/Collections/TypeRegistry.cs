using System.Collections.Concurrent;

namespace Servus.Core.Collections;

public class TypeRegistry<TValue>
{
    protected readonly ConcurrentDictionary<Type, TValue> Dictionary = [];

    public void Add<TKey>(TValue value) => Add(typeof(TKey), value);

    public void Add(Type key, TValue value) 
        => Dictionary.AddOrUpdate(key, value, (_, _) => value);

    public TValue Get<TKey>() => Get(typeof(TKey));

    public TValue Get(Type key)
    {
        if (!Dictionary.TryGetValue(key, out var value)) throw new KeyNotFoundException();
        return value;
    }

    public TValue GetOrAdd<TKey>(Func<TValue> factory) => GetOrAdd(typeof(TKey), factory);

    public TValue GetOrAdd(Type key, Func<TValue> factory)
        => Dictionary.GetOrAdd(key, (_) => factory());
}