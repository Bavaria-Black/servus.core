using System.Runtime.CompilerServices;

namespace Servus.Core;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items) => items.ForEach(collection.Add);

    public static bool IsEmpty<T>(this IReadOnlyCollection<T> enumerable) => enumerable.Count == 0;
}