using System.Runtime.CompilerServices;

namespace Servus.Core;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// High-performance insert at specified position. Optimized for collections with known count.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable</typeparam>
    /// <param name="source">The source enumerable</param>
    /// <param name="index">The zero-based index at which to insert the item</param>
    /// <param name="item">The item to insert</param>
    /// <returns>A new enumerable with the item inserted at the specified position</returns>
    public static IEnumerable<T> InsertAt<T>(this IEnumerable<T> source, int index, T item) => InsertAt(source, index, [item]);
    
    /// <summary>
    /// High-performance insert at specified position. Optimized for collections with known count.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable</typeparam>
    /// <param name="source">The source enumerable</param>
    /// <param name="index">The zero-based index at which to insert the item</param>
    /// <param name="item">The item to insert</param>
    /// <returns>A new enumerable with the item inserted at the specified position</returns>
    public static IEnumerable<T> InsertAt<T>(this IEnumerable<T> source, int index, params T[] item)
    {
        var list = source.ToList();
        list.InsertRange(index, item);
        return list;
    }

    /// <summary>
    /// Ultra-fast insert for arrays with minimal bounds checking.
    /// Optimized for performance when you know the index is likely valid.
    /// </summary>
    /// <typeparam name="T">The type of elements</typeparam>
    /// <param name="array">The source array</param>
    /// <param name="index">The insertion index</param>
    /// <param name="item">The item to insert</param>
    /// <returns>New array with item inserted</returns>
    public static T[] InsertAt<T>(this T[] array, int index, T item) => InsertAt(array, index, [item]);

    /// <summary>
    /// Ultra-fast insert for arrays with minimal bounds checking.
    /// Optimized for performance when you know the index is likely valid.
    /// </summary>
    /// <typeparam name="T">The type of elements</typeparam>
    /// <param name="array">The source array</param>
    /// <param name="index">The insertion index</param>
    /// <param name="item">The item to insert</param>
    /// <returns>New array with item inserted</returns>
    public static T[] InsertAt<T>(this T[] array, int index, params T[] item)
    {
        var itemsLength = item.Length;
        var length = array.Length;
        var result = new T[length + item.Length];

        // Copy elements before insertion point
        Array.Copy(array, 0, result, 0, index);

        // Copy the items to insert
        Array.Copy(item, 0, result, index, itemsLength);

        // Copy elements after insertion point
        Array.Copy(array, index, result, index + itemsLength, length - index);

        return result;
    }
}