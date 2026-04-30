using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Servus.Core.Diagnostics;

/// <summary>
/// Static API for zero-cost developer tracing. When no listener is configured,
/// trace calls are no-ops (single null-check + inlined branch).
/// <see cref="Configure"/> is called once at startup before any worker threads exist,
/// so the thread-creation happens-before guarantees visibility without barriers.
/// </summary>
public class ServusTrace
{
    private TraceConfig? _config;
    private readonly ConcurrentDictionary<string, TraceChannel> _channels = new();
    public ActivitySource Source { get; } = new("Servus", ServusInfo.Version);

    internal ServusTrace()
    {
        
    }
    
    /// <summary>
    /// Enables tracing with the specified listener, minimum level, and optional category filter.
    /// Pass a predicate to restrict tracing to categories for which the predicate returns <see langword="true"/>;
    /// omit the filter to enable all categories. If you have a list or set of categories, build a predicate
    /// from it (for example, <c>category => allowedCategories.Contains(category)</c>).
    /// Must be called before the Akka actor system starts — thread creation provides
    /// happens-before visibility to all worker threads.
    /// </summary>
    /// <param name="listener">The trace listener that receives enabled trace events.</param>
    /// <param name="minimumLevel">The minimum trace level to emit.</param>
    /// <param name="categoryFilter">
    /// An optional predicate that determines whether tracing is enabled for a category.
    /// If <see langword="null"/>, tracing is enabled for all categories.
    /// </param>
    public void Configure(
        IServusTraceListener listener,
        TraceLevel minimumLevel = TraceLevel.Trace,
        Func<string, bool>? categoryFilter = null)
    {
        _config = new TraceConfig(listener, categoryFilter ?? (_ => true), minimumLevel);
    }

    /// <summary>
    /// Creates a <see cref="TraceChannel"/> for a custom category.
    /// Store the result in a static field for zero-allocation reuse:
    /// <code>private static readonly ServusTraceChannel _http = ServusTrace.For("Http");</code>
    /// </summary>
    public TraceChannel For(string categoryName) =>
        _channels.GetOrAdd(categoryName, static name => new TraceChannel(name));

    /// <summary>
    /// Disables tracing. All subsequent trace calls become no-ops.
    /// </summary>
    public void Disable()
    {
        _config = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool ShouldTrace(string category, TraceLevel level)
    {
        var cfg = _config;
        if (cfg is null) return false;
        if (level < cfg.MinimumLevel) return false;
        
        return cfg.CategoryFilter.Invoke(category) 
               && cfg.Listener.IsEnabled(level, category);
    }
    
    public void Trace<T>(T source, TraceLevel traceLevel, string category, string message, long? ticks = null, params object?[] args)
    {
        if (!ShouldTrace(category, traceLevel) || source is null) return;
        WriteEvent(new TraceEvent(ticks ?? Stopwatch.GetTimestamp(), traceLevel, category,
            TypeNameCache<T>.Name, source.GetHashCode(), message, args));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void WriteEvent(in TraceEvent evt)
    {
        _config?.Listener.Write(in evt);
    }

    private static class TypeNameCache<T>
    {
        public static readonly string Name = typeof(T).Name;
    }

    private sealed record TraceConfig(
        IServusTraceListener Listener,
        Func<string, bool> CategoryFilter,
        TraceLevel MinimumLevel);
}
