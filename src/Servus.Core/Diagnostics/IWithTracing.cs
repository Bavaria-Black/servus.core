using System.Diagnostics;

namespace Servus.Core.Diagnostics;

public interface IWithTracing
{
    string? TraceId { get; set; }
    string? SpanId { get; set; }

    ActivityContext GetContext()
    {
        return new ActivityContext(
            ActivityTraceId.CreateFromString(string.IsNullOrEmpty(TraceId)? ActivityTraceId.CreateRandom().ToHexString() : TraceId),
            ActivitySpanId.CreateFromString(string.IsNullOrEmpty(SpanId) ? ActivitySpanId.CreateRandom().ToHexString() : SpanId),
            ActivityTraceFlags.Recorded);
    }

    void AddTracing() => AddTracing(Activity.Current?.TraceId.ToHexString(), Activity.Current?.SpanId.ToHexString());
    void AddTracing(IWithTracing tracing) => AddTracing(tracing.TraceId, tracing.SpanId);

    void AddTracing(string? traceId, string? spanId)
    {
        TraceId = string.IsNullOrEmpty(traceId) ? ActivityTraceId.CreateRandom().ToHexString() : traceId;
        SpanId = string.IsNullOrEmpty(spanId) ? ActivitySpanId.CreateRandom().ToHexString() : spanId;
    }

    Activity? StartActivity(string name, ActivitySource source, ActivityKind kind = ActivityKind.Consumer)
        => source.StartActivity(name, kind, GetContext());
}