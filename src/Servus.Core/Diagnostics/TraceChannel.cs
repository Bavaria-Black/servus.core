using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Servus.Core.Diagnostics;

/// <summary>
/// A trace channel bound to a single category.
/// Obtain channels with <see cref="ServusTrace.For(string)"/>.
/// </summary>
public readonly struct TraceChannel(string category)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trace(object source, string message)
    {
        if (!ServusTrace.ShouldTrace(category, TraceLevel.Trace)) return;
        ServusTrace.WriteEvent(new TraceEvent(Stopwatch.GetTimestamp(), TraceLevel.Trace, category,
            source.GetType().Name, source.GetHashCode(), message));
    }

    public void Trace(object source, string message, params object?[] args)
    {
        if (!ServusTrace.ShouldTrace(category, TraceLevel.Trace)) return;
        ServusTrace.WriteEvent(new TraceEvent(Stopwatch.GetTimestamp(), TraceLevel.Trace, category,
            source.GetType().Name, source.GetHashCode(), message, args));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(object source, string message)
    {
        if (!ServusTrace.ShouldTrace(category, TraceLevel.Debug)) return;
        ServusTrace.WriteEvent(new TraceEvent(Stopwatch.GetTimestamp(), TraceLevel.Debug, category,
            source.GetType().Name, source.GetHashCode(), message));
    }

    public void Debug(object source, string message, params object?[] args)
    {
        if (!ServusTrace.ShouldTrace(category, TraceLevel.Debug)) return;
        ServusTrace.WriteEvent(new TraceEvent(Stopwatch.GetTimestamp(), TraceLevel.Debug, category,
            source.GetType().Name, source.GetHashCode(), message, args));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(object source, string message)
    {
        if (!ServusTrace.ShouldTrace(category, TraceLevel.Info)) return;
        ServusTrace.WriteEvent(new TraceEvent(Stopwatch.GetTimestamp(), TraceLevel.Info, category,
            source.GetType().Name, source.GetHashCode(), message));
    }

    public void Info(object source, string message, params object?[] args)
    {
        if (!ServusTrace.ShouldTrace(category, TraceLevel.Info)) return;
        ServusTrace.WriteEvent(new TraceEvent(Stopwatch.GetTimestamp(), TraceLevel.Info, category,
            source.GetType().Name, source.GetHashCode(), message, args));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warning(object source, string message)
    {
        if (!ServusTrace.ShouldTrace(category, TraceLevel.Warning)) return;
        ServusTrace.WriteEvent(new TraceEvent(Stopwatch.GetTimestamp(), TraceLevel.Warning, category,
            source.GetType().Name, source.GetHashCode(), message));
    }

    public void Warning(object source, string message, params object?[] args)
    {
        if (!ServusTrace.ShouldTrace(category, TraceLevel.Warning)) return;
        ServusTrace.WriteEvent(new TraceEvent(Stopwatch.GetTimestamp(), TraceLevel.Warning, category,
            source.GetType().Name, source.GetHashCode(), message, args));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(object source, string message)
    {
        if (!ServusTrace.ShouldTrace(category, TraceLevel.Error)) return;
        ServusTrace.WriteEvent(new TraceEvent(Stopwatch.GetTimestamp(), TraceLevel.Error, category,
            source.GetType().Name, source.GetHashCode(), message));
    }

    public void Error(object source, string message, params object?[] args)
    {
        if (!ServusTrace.ShouldTrace(category, TraceLevel.Error)) return;
        ServusTrace.WriteEvent(new TraceEvent(Stopwatch.GetTimestamp(), TraceLevel.Error, category,
            source.GetType().Name, source.GetHashCode(), message, args));
    }
}
