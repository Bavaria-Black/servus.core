using System.Diagnostics;

namespace Servus.Core.Diagnostics;

public interface IWithTracing
{
    [Obsolete("Use TraceParent property instead. TraceId is now part of the W3C Trace Context standard format.")]
    string? TraceId
    {
        get => null;
        set => _ = value;
    }

    [Obsolete("Use TraceParent property instead. SpanId is now part of the W3C Trace Context standard format.")]
    string? SpanId
    {
        get => null;
        set => _ = value;
    }

    string? TraceParent
    {
        get => null;
        set => _ = value;
    }

    string? TraceState
    {
        get => null;
        set => _ = value;
    }

    ActivityContext GetContext()
    {
        if (ActivityContext.TryParse(TraceParent, TraceState, out var context))
        {
            return context;
        }

        var traceId = ActivityTraceId.CreateFromString(string.IsNullOrEmpty(TraceId)
            ? ActivityTraceId.CreateRandom().ToHexString()
            : TraceId);
        var spanId =
            ActivitySpanId.CreateFromString(string.IsNullOrEmpty(SpanId)
                ? ActivitySpanId.CreateRandom().ToHexString()
                : SpanId);
        return new ActivityContext(traceId, spanId, ActivityTraceFlags.Recorded);
    }

    void AddTracing() => AddTracing(Activity.Current);

    void AddTracing(IWithTracing tracing)
        => AddTracing(traceParent: tracing.TraceParent, traceState: tracing.TraceState);


    void AddTracing(Activity? activity)
    {
        TraceParent ??= activity?.Id;
        TraceState ??= activity?.TraceStateString;
    }

    void AddTracing(string? traceParent, ReadOnlySpan<char> traceState = default)
    {
        TraceParent ??= traceParent;
        TraceState ??= traceState.ToString();
    }

    [Obsolete("Use W3C traceparent/tracestate")]
    void AddTracing(string? traceId, string? spanId)
    {
        TraceId = string.IsNullOrEmpty(traceId) ? ActivityTraceId.CreateRandom().ToHexString() : traceId;
        SpanId = string.IsNullOrEmpty(spanId) ? ActivitySpanId.CreateRandom().ToHexString() : spanId;
    }

    Activity? StartActivity(string name, ActivitySource source, ActivityKind kind = ActivityKind.Consumer)
        => source.StartActivity(name, kind, GetContext());
}