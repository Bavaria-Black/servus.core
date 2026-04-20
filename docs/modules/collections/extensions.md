# Collection Extensions

A grab-bag of extension methods for `IEnumerable<T>`, `ICollection<T>`, `IReadOnlyCollection<T>`, and `IAsyncEnumerable<T>`. Most of them are little one-liners you'd otherwise write every project.

## `EnumerableExtensions`

### `DistinctBy`

Keep the first element per key:

```csharp
using Servus.Core;

var firstPerCountry = users.DistinctBy(u => u.Country);
```

> Note: .NET 6 added a built-in `DistinctBy`. Servus.Core's version predates it and stays for compatibility.

### `GetIndex`

Index of the first match, or `-1`:

```csharp
var firstErrorAt = logs.GetIndex(e => e.Level == LogLevel.Error);
```

### `ForEach` and `ForEachIndexed`

Run an action per item. The indexed variant gives you the position.

```csharp
users.ForEach(u => Console.WriteLine(u.Name));

users.ForEachIndexed((u, i) => Console.WriteLine($"{i}: {u.Name}"));
```

### `Contains(predicate)`

Predicate form of `Contains`:

```csharp
bool hasOverdue = orders.Contains(o => o.DueDate < DateTime.UtcNow);
```

### `TryGet`

Non-throwing first-match lookup:

```csharp
if (orders.TryGet(o => o.Id == id, out var order))
{
    Process(order);
}
```

## `EnumerableExtensions.Insert` — `InsertAt` / `InsertRangeAt`

Insert a single item or multiple items at a specific index. Works on both `IEnumerable<T>` and `T[]`:

```csharp
int[] original = [1, 2, 4, 5];
int[] fixed_ = original.InsertAt(2, 3);
// fixed_ = [1, 2, 3, 4, 5]

int[] many = original.InsertRangeAt(2, 3, 3, 3);
// many = [1, 2, 3, 3, 3, 4, 5]

IEnumerable<string> words = ["hello", "world"];
var greeting = words.InsertAt(1, "beautiful");
// greeting = ["hello", "beautiful", "world"]
```

The array overloads are optimised paths that avoid enumeration.

## `CollectionExtensions`

### `AddRange<T>(this ICollection<T>, IEnumerable<T>)`

Batch-add into any `ICollection<T>`; `List<T>` already has this, generic `ICollection<T>` does not.

```csharp
using Servus.Core;

ICollection<int> items = new HashSet<int>();
items.AddRange([1, 2, 3, 4]);
```

### `IsEmpty<T>(this IReadOnlyCollection<T>)`

Readable alternative to `Count == 0`:

```csharp
if (pendingOrders.IsEmpty())
{
    return;
}
```

## `AsyncEnumerableExtensions`

LINQ-like short-circuiting helpers over `IAsyncEnumerable<T>`.

```csharp
using Servus.Core.Collections;

// Returns true on the first matching element
bool hasFailure = await healthStream.AnyAsync(h => !h.IsHealthy);

// Returns false on the first non-matching element
bool allHealthy = await healthStream.AllAsync(h => h.IsHealthy);

// Straight bool streams
bool anyTrue  = await boolStream.AnyAsync();
bool allTrue  = await boolStream.AllAsync();
```

## API summary

```csharp
// Servus.Core
public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items);
    public static bool IsEmpty<T>(this IReadOnlyCollection<T> enumerable);
}

public static partial class EnumerableExtensions
{
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property);
    public static int GetIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate);
    public static void ForEachIndexed<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action);
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action);
    public static bool Contains<T>(this IEnumerable<T> enumerable, Predicate<T> predicate);
    public static bool TryGet<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, out T? entry);

    public static IEnumerable<T> InsertAt<T>(this IEnumerable<T> source, int index, T item);
    public static T[] InsertAt<T>(this T[] array, int index, T item);
    public static T[] InsertRangeAt<T>(this T[] array, int index, params T[] items);
    public static IEnumerable<T> InsertRangeAt<T>(this IEnumerable<T> source, int index, params T[] items);
}

// Servus.Core.Collections
public static class AsyncEnumerableExtensions
{
    public static Task<bool> AnyAsync(this IAsyncEnumerable<bool> enumerable);
    public static Task<bool> AnyAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, bool> predicate);
    public static Task<bool> AllAsync(this IAsyncEnumerable<bool> enumerable);
    public static Task<bool> AllAsync<T>(this IAsyncEnumerable<T> enumerable, Func<T, bool> predicate);
}
```

## `InsertionBehavior`

An enum used by some Servus types (e.g. [`StringConverterCollection`](/modules/type-system/string-converters)) to decide what happens when an item with the same key already exists:

```csharp
public enum InsertionBehavior
{
    None = 0,                   // silently skip
    OverwriteExisting = 1,      // replace existing value
    ThrowOnExisting = 2         // throw if key exists
}
```
