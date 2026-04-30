using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Servus.Core.Diagnostics;

/// <summary>
/// A trace channel bound to a single category.
/// Obtain channels with <see cref="ServusTrace.For(string)"/>.
/// </summary>
public readonly struct ServusTraceChannel(string category)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trace(object source, string message)
    {
        if (!ServusTrace.ShouldTrace(category, ServusTraceLevel.Trace)) return;
        ServusTrace.WriteEvent(new ServusTraceEvent(Stopwatch.GetTimestamp(), ServusTraceLevel.Trace, category,
            source.GetType().Name, source.GetHashCode(), message));
    }

    public void Trace(object source, string message, params object?[] args)
    {
        if (!ServusTrace.ShouldTrace(category, ServusTraceLevel.Trace)) return;
        ServusTrace.WriteEvent(new ServusTraceEvent(Stopwatch.GetTimestamp(), ServusTraceLevel.Trace, category,
            source.GetType().Name, source.GetHashCode(), message, args));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(object source, string message)
    {
        if (!ServusTrace.ShouldTrace(category, ServusTraceLevel.Debug)) return;
        ServusTrace.WriteEvent(new ServusTraceEvent(Stopwatch.GetTimestamp(), ServusTraceLevel.Debug, category,
            source.GetType().Name, source.GetHashCode(), message));
    }

    public void Debug(object source, string message, params object?[] args)
    {
        if (!ServusTrace.ShouldTrace(category, ServusTraceLevel.Debug)) return;
        ServusTrace.WriteEvent(new ServusTraceEvent(Stopwatch.GetTimestamp(), ServusTraceLevel.Debug, category,
            source.GetType().Name, source.GetHashCode(), message, args));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(object source, string message)
    {
        if (!ServusTrace.ShouldTrace(category, ServusTraceLevel.Info)) return;
        ServusTrace.WriteEvent(new ServusTraceEvent(Stopwatch.GetTimestamp(), ServusTraceLevel.Info, category,
            source.GetType().Name, source.GetHashCode(), message));
    }

    public void Info(object source, string message, params object?[] args)
    {
        if (!ServusTrace.ShouldTrace(category, ServusTraceLevel.Info)) return;
        ServusTrace.WriteEvent(new ServusTraceEvent(Stopwatch.GetTimestamp(), ServusTraceLevel.Info, category,
            source.GetType().Name, source.GetHashCode(), message, args));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warning(object source, string message)
    {
        if (!ServusTrace.ShouldTrace(category, ServusTraceLevel.Warning)) return;
        ServusTrace.WriteEvent(new ServusTraceEvent(Stopwatch.GetTimestamp(), ServusTraceLevel.Warning, category,
            source.GetType().Name, source.GetHashCode(), message));
    }

    public void Warning(object source, string message, params object?[] args)
    {
        if (!ServusTrace.ShouldTrace(category, ServusTraceLevel.Warning)) return;
        ServusTrace.WriteEvent(new ServusTraceEvent(Stopwatch.GetTimestamp(), ServusTraceLevel.Warning, category,
            source.GetType().Name, source.GetHashCode(), message, args));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(object source, string message)
    {
        if (!ServusTrace.ShouldTrace(category, ServusTraceLevel.Error)) return;
        ServusTrace.WriteEvent(new ServusTraceEvent(Stopwatch.GetTimestamp(), ServusTraceLevel.Error, category,
            source.GetType().Name, source.GetHashCode(), message));
    }

    public void Error(object source, string message, params object?[] args)
    {
        if (!ServusTrace.ShouldTrace(category, ServusTraceLevel.Error)) return;
        ServusTrace.WriteEvent(new ServusTraceEvent(Stopwatch.GetTimestamp(), ServusTraceLevel.Error, category,
            source.GetType().Name, source.GetHashCode(), message, args));
    }
}
