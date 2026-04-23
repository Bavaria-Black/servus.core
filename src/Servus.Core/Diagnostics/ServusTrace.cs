using System.Runtime.CompilerServices;

namespace Servus.Core.Diagnostics;

/// <summary>
/// Static API for zero-cost developer tracing. When no listener is configured,
/// trace calls are no-ops (single null-check + inlined branch).
/// <see cref="Configure"/> is called once at startup before any worker threads exist,
/// so the thread-creation happens-before guarantees visibility without barriers.
/// </summary>
public static class ServusTrace
{
    private static TraceConfig? _config;

    /// <summary>
    /// Enables tracing with the specified listener and optional category filter.
    /// Pass one or more categories to restrict tracing; omit to enable all categories.
    /// Must be called before the Akka actor system starts — thread creation provides
    /// happens-before visibility to all worker threads.
    /// </summary>
    public static void Configure(
        IServusTraceListener listener,
        ServusTraceLevel minimumLevel = ServusTraceLevel.Trace,
        Func<ServusTraceCategory, bool>? categoryFilter = null)
    {
        _config = new TraceConfig(listener, categoryFilter ?? (_ => true), minimumLevel);
    }

    /// <summary>
    /// Creates a <see cref="ServusTraceChannel"/> for a custom category.
    /// Store the result in a static field for zero-allocation reuse:
    /// <code>private static readonly ServusTraceChannel _http = ServusTrace.For(new TraceCategory("Http"));</code>
    /// </summary>
    public static ServusTraceChannel For(ServusTraceCategory category) => new(category);

    public static ServusTraceChannel For(string categoryName) => new(new ServusTraceCategory(categoryName));

    /// <summary>
    /// Disables tracing. All subsequent trace calls become no-ops.
    /// </summary>
    public static void Disable()
    {
        _config = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ShouldTrace(ServusTraceCategory category, ServusTraceLevel level)
    {
        var cfg = _config;
        if (cfg is null) return false;
        if (level < cfg.MinimumLevel) return false;
        if (!cfg.CategoryFilter.Invoke(category)) return false;
        return cfg.Listener.IsEnabled(level, category);
    }

    internal static void WriteEvent(in ServusTraceEvent evt)
    {
        _config?.Listener.Write(in evt);
    }

    private sealed record TraceConfig(
        IServusTraceListener Listener,
        Func<ServusTraceCategory, bool> CategoryFilter,
        ServusTraceLevel MinimumLevel);
}