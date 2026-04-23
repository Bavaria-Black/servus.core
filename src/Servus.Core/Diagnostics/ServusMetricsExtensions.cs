using System.Diagnostics.Metrics;

namespace Servus.Core.Diagnostics;

/// <summary>
/// Built-in HTTP-client instrument factories for <see cref="ServusMetrics"/>.
/// </summary>
public static class ServusMetricsExtensions
{
    /// <summary>
    /// Number of open connections.
    /// Tags: <c>http.connection.state</c> (<c>"active"</c> or <c>"idle"</c>),
    /// <c>server.address</c>, <c>server.port</c>.
    /// </summary>
    public static UpDownCounter<long> AddOpenConnections(this ServusMetrics metrics)
        => metrics.Meter.CreateUpDownCounter<long>(
            "http.client.open_connections",
            unit: "{connection}",
            description: "Number of currently open transport connections");

    /// <summary>
    /// Connection lifetime in seconds.
    /// Tags: <c>server.address</c>, <c>server.port</c>.
    /// </summary>
    public static Histogram<double> AddConnectionDuration(this ServusMetrics metrics)
        => metrics.Meter.CreateHistogram<double>(
            "http.client.connection.duration",
            unit: "s",
            description: "Duration of transport connections in seconds");

    /// <summary>
    /// Time spent waiting for a connection from the pool, in seconds.
    /// Tags: <c>server.address</c>, <c>server.port</c>.
    /// </summary>
    public static Histogram<double> AddRequestTimeInQueue(this ServusMetrics metrics)
        => metrics.Meter.CreateHistogram<double>(
            "http.client.request.time_in_queue",
            unit: "s",
            description: "Time spent waiting for a connection from the pool");

    /// <summary>
    /// Duration of DNS lookups, in seconds.
    /// Tags: <c>dns.question.name</c>, <c>error.type</c> (if failed).
    /// </summary>
    public static Histogram<double> AddDnsLookupDuration(this ServusMetrics metrics)
        => metrics.Meter.CreateHistogram<double>(
            "dns.lookup.duration",
            unit: "s",
            description: "Duration of DNS lookups");
}