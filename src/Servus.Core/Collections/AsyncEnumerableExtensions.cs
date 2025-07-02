namespace Servus.Core.Collections;

public static class AsyncEnumerableExtensions
{
    /// <summary>
    /// Determines whether any element in the async enumerable sequence is true.
    /// </summary>
    /// <param name="enumerable">The async enumerable sequence of boolean values to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if any element is true; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when enumerable is null.</exception>
    public static async Task<bool> AnyAsync(this IAsyncEnumerable<bool> enumerable)
        => await AnyAsync(enumerable, e => e);

    /// <summary>
    /// Determines whether any element in the async enumerable sequence satisfies the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="enumerable">The async enumerable sequence to check.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if any element satisfies the predicate; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when enumerable or predicate is null.</exception>
    /// <remarks>
    /// This method stops enumeration as soon as an element satisfying the predicate is found, providing short-circuit evaluation.
    /// </remarks>
    public static async Task<bool> AnyAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        var result = false;
        await using var e = enumerable.GetAsyncEnumerator();
        while (!result && await e.MoveNextAsync()) result = predicate(e.Current);

        return result;
    }
    
    /// <summary>
    /// Determines whether all elements in the async enumerable sequence are true.
    /// </summary>
    /// <param name="enumerable">The async enumerable sequence of boolean values to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if all elements are true or if the sequence is empty; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when enumerable is null.</exception>
    public static async Task<bool> AllAsync(this IAsyncEnumerable<bool> enumerable)
        => await AllAsync(enumerable, e => e);

    /// <summary>
    /// Determines whether all elements in the async enumerable sequence satisfy the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="enumerable">The async enumerable sequence to check.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if all elements satisfy the predicate or if the sequence is empty; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when enumerable or predicate is null.</exception>
    /// <remarks>
    /// This method stops enumeration as soon as an element not satisfying the predicate is found, providing short-circuit evaluation.
    /// </remarks>
    public static async Task<bool> AllAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        var result = true;
        await using var e = enumerable.GetAsyncEnumerator();
        while (result && await e.MoveNextAsync()) result = predicate(e.Current);

        return result;
    }
}