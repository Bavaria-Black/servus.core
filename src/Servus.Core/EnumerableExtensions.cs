namespace Servus.Core;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Selects items from an IEnumerable which are distinct by the given property / properties.
    /// </summary>
    /// <typeparam name="T">The element type of the IEnumerable</typeparam>
    /// <typeparam name="TKey">The type of the key</typeparam>
    /// <param name="items">The IEnumerable to select distinct items from</param>
    /// <param name="property">The property / properties of T which will be taken into account when checking for distinctness</param>
    /// <returns>An IEnumerable which is distinct by the given property / properties</returns>
    /// <example>
    /// This sample shows how to call the <see cref="DistinctBy{T, TKey}" /> method.
    /// <code>
    /// var list = new List{Point}();
    /// var distinct = list.DistinctBy(p => p.X);
    /// </code>
    /// You can also select multiple properties of the item type T like this:
    /// <code>
    /// var list = new List{Point}();
    /// var distinct = list.DistinctBy(p => new { p.X, p.Y });
    /// </code>
    /// </example>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
    {
        return items.GroupBy(property).Select(x => x.First());
    }

    public static int GetIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        
        var index = 0;
        var enumerator = source.GetEnumerator();
        using var disposable = enumerator as IDisposable;
        while (enumerator.MoveNext())
        {
            if (predicate(enumerator.Current)) return index;
            index++;
        }

        return -1;
    }

    public static void ForEachIndexed<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);
        
        var index = 0;
        source.ForEach(a => action(a, index++));
    }

    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in source)
        {
            action(item);
        }
    }
    
    public static bool Contains<T>(this IEnumerable<T> enumerable, Predicate<T> predicate) => enumerable.Any(a => predicate(a));

    public static bool TryGet<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, out T? entry)
    {
        entry = enumerable.FirstOrDefault(e => predicate(e));
        return entry is not null;
    }

}