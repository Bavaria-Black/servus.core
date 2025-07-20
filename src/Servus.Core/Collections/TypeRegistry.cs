using System.Collections.Concurrent;

namespace Servus.Core.Collections;

public class TypeRegistry<TValue>
{
    protected readonly ConcurrentDictionary<Type, TValue> Dictionary = [];

    public void Add<TKey>(TValue value)
    {
        Dictionary.AddOrUpdate(typeof(TKey), value, (_, _) => value);
    }

    public TValue Get<TKey>()
    {
        if (!Dictionary.TryGetValue(typeof(TKey), out var value)) throw new KeyNotFoundException();
        return value;
    }

    public TValue GetOrAdd<TKey>(Func<TValue> factory)
    {
        return Dictionary.GetOrAdd(typeof(TKey), (_) => factory());
    }
}