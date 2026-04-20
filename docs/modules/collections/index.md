# Collections

A handful of data structures and LINQ-like extensions that the BCL doesn't give you out of the box. Everything here is either (a) a collection with a specific behaviour you'd otherwise reinvent, or (b) a tiny extension that saves you a one-liner you write on every project.

## Pages in this section

- [**Circular Queue**](./circular-queue) — fixed-capacity FIFO that drops the oldest item when full. Ring buffer without the fuss.
- [**Handler Registry**](./handler-registry) — predicate/action pairs with first-match or all-match dispatch and a stash/pop mechanism for testing.
- [**Type Registry**](./type-registry) — `Type`-keyed dictionary with `GetOrAdd` semantics; the backbone of pluggable systems.
- [**Lazy Value Cache**](./lazy-value-cache) — memory-cached lazy initialisation with per-entry expiration.
- [**Extensions**](./extensions) — `EnumerableExtensions`, `CollectionExtensions`, `AsyncEnumerableExtensions` and the `InsertAt` helpers.

## Namespace map

| Namespace | Types |
|---|---|
| `Servus.Core.Collections` | `CircularQueue<T>`, `HandlerRegistry`, `TypeRegistry<TValue>`, `LazyValueCache<TKey,TValue>`, `InsertionBehavior`, `AsyncEnumerableExtensions` |
| `Servus.Core` | `CollectionExtensions`, `EnumerableExtensions`, `EnumerableExtensions.Insert` |
