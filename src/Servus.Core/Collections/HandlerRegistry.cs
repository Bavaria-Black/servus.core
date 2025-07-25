namespace Servus.Core.Collections;

public class HandlerRegistry
{
    private readonly List<HandlerEntry> _handlers = [];
    private readonly Stack<List<HandlerEntry>> _stash = [];

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="handler">The action to execute when the type matches</param>
    public void Register<T>(Action<T> handler) => Register(_ => true, handler);

    /// <summary>
    /// Registers a handler with its associated condition.
    /// </summary>
    /// <param name="canHandle">The condition that determines if the handler should be executed</param>
    /// <param name="handler">The action to execute when the condition matches</param>
    public void Register<T>(Predicate<T> canHandle, Action<T> handler)
    {
        var (predicate, action) = Wrap(canHandle, handler);
        Register(typeof(T), predicate, action);
    }

    /// <summary>
    /// Registers a handler with its associated condition.
    /// </summary>
    /// <param name="type">The type of the object to handle</param>
    /// <param name="canHandle">The condition that determines if the handler should be executed</param>
    /// <param name="handler">The action to execute when the condition matches</param>
    public void Register(Type type, Predicate<object> canHandle, Action<object> handler)
    {
        ArgumentNullException.ThrowIfNull(canHandle);
        ArgumentNullException.ThrowIfNull(handler);
        
        _handlers.Add(new HandlerEntry(type, canHandle, handler));
    }

    private static (Predicate<object> canHandle, Action<object> handler) Wrap<T>(Predicate<T> canHandle,
        Action<T> handler)
    {
        ArgumentNullException.ThrowIfNull(canHandle);
        ArgumentNullException.ThrowIfNull(handler);
        return (o => canHandle((T) o), o => handler((T) o));
    }

    /// <summary>
    /// Handles the item by executing only the first registered handler whose condition matches.
    /// </summary>
    /// <param name="item">The item to handle</param>
    /// <returns>True if a handler was executed, false otherwise</returns>
    public bool Handle(object item)
    {
        var handler = GetMatchingHandlers(item).FirstOrDefault();

        if (handler == null) return false;
        handler(item);
        return true;
    }

    /// <summary>
    /// Handles the item by executing all registered handlers whose conditions match.
    /// </summary>
    /// <param name="item">The item to handle</param>
    /// <returns>The number of handlers that were executed</returns>
    public int HandleAll(object item)
    {
        var matchingHandlers = GetMatchingHandlers(item).ToList();

        foreach (var entry in matchingHandlers)
        {
            entry(item);
        }

        return matchingHandlers.Count;
    }

    /// <summary>
    /// Checks if any registered handler can handle the given item.
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns>True if at least one handler can handle the item</returns>
    public bool CanHandle(object item)
    {
        return GetMatchingHandlers(item).Any();
    }

    /// <summary>
    /// Gets the number of registered handlers.
    /// </summary>
    public int Count => _handlers.Count;

    /// <summary>
    /// Removes all registered handlers and clears the stash.
    /// </summary>
    public void Clear()
    {
        _handlers.Clear();
        _stash.Clear();
    }

    /// <summary>
    /// Gets all handlers whose predicates match the given item.
    /// </summary>
    /// <param name="item">The item to match against</param>
    /// <returns>An enumerable of matching handlers</returns>
    public IEnumerable<Action<object>> GetMatchingHandlers(object item)
    {
        return _handlers
            .Where(i => i.Type.IsAssignableTo(item.GetType()))
            .Where(entry => entry.CanHandle(item))
            .Select(entry => entry.Handler);
    }

    /// <summary>
    /// Stashes the current handlers and starts with a clean registry.
    /// </summary>
    public void Stash()
    {
        _stash.Push([.._handlers]);
        _handlers.Clear();
    }

    /// <summary>
    /// Restores the most recently stashed handlers, replacing current ones.
    /// </summary>
    /// <returns>True if handlers were restored, false if stash was empty</returns>
    public bool Pop()
    {
        if (_stash.Count == 0)
            return false;

        _handlers.Clear();
        _handlers.AddRange(_stash.Pop());
        return true;
    }

    private sealed record HandlerEntry(Type Type, Predicate<object> CanHandle, Action<object> Handler);
}