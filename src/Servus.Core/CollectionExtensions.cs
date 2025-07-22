namespace Servus.Core;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items) => items.ForEach(collection.Add);
}