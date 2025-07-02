namespace Servus.Core.Enumerables;

public static class AsyncEnumerableExtensions
{
    public static async Task<bool> AnyAsync(this IAsyncEnumerable<bool> enumerable)
        => await AnyAsync(enumerable, e => e);

    public static async Task<bool> AnyAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        var result = false;
        await using var e = enumerable.GetAsyncEnumerator();
        while (!result && await e.MoveNextAsync()) result = predicate(e.Current);

        return result;
    }
    
    public static async Task<bool> AllAsync(this IAsyncEnumerable<bool> enumerable)
        => await AllAsync(enumerable, e => e);

    public static async Task<bool> AllAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        var result = true;
        await using var e = enumerable.GetAsyncEnumerator();
        while (result && await e.MoveNextAsync()) result = predicate(e.Current);

        return result;
    }
}