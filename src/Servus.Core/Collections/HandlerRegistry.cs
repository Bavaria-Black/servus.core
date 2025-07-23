using System;
using System.Collections.Generic;
using System.Linq;

public class HandlerRegistry<T>
{
    private readonly List<HandlerEntry> _handlers = new List<HandlerEntry>();
    private readonly Stack<List<HandlerEntry>> _stash = new Stack<List<HandlerEntry>>();

    /// <summary>
    /// Registers a handler with its associated condition.
    /// </summary>
    /// <param name="canHandle">The condition that determines if the handler should be executed</param>
    /// <param name="handler">The action to execute when the condition matches</param>
    public void Register(Predicate<T> canHandle, Action<T> handler)
    {
        ArgumentNullException.ThrowIfNull(canHandle);
        ArgumentNullException.ThrowIfNull(handler);

        _handlers.Add(new HandlerEntry(canHandle, handler));
    }

    /// <summary>
    /// Handles the item by executing only the first registered handler whose condition matches.
    /// </summary>
    /// <param name="item">The item to handle</param>
    /// <returns>True if a handler was executed, false otherwise</returns>
    public bool Handle(T item)
    {
        var handler = _handlers.FirstOrDefault(entry => entry.CanHandle(item));
        
        if (handler != null)
        {
            handler.Handler(item);
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Handles the item by executing all registered handlers whose conditions match.
    /// </summary>
    /// <param name="item">The item to handle</param>
    /// <returns>The number of handlers that were executed</returns>
    public int HandleAll(T item)
    {
        var matchingHandlers = _handlers.Where(entry => entry.CanHandle(item)).ToList();
        
        foreach (var entry in matchingHandlers)
        {
            entry.Handler(item);
        }
        
        return matchingHandlers.Count;
    }

    /// <summary>
    /// Checks if any registered handler can handle the given item.
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns>True if at least one handler can handle the item</returns>
    public bool CanHandle(T item)
    {
        return _handlers.Any(entry => entry.CanHandle(item));
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
    public IEnumerable<Action<T>> GetMatchingHandlers(T item)
    {
        return _handlers
            .Where(entry => entry.CanHandle(item))
            .Select(entry => entry.Handler);
    }

    /// <summary>
    /// Stashes the current handlers and starts with a clean registry.
    /// </summary>
    public void Stash()
    {
        _stash.Push(new List<HandlerEntry>(_handlers));
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

    private record HandlerEntry(Predicate<T> CanHandle, Action<T> Handler);
}