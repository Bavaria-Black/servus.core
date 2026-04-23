using System.Diagnostics.Metrics;
using System.Reflection;

namespace Servus.Core.Diagnostics;

/// <summary>
/// OpenTelemetry metrics for the Servus.Akka transport layer.
/// Tracks connection lifecycle, DNS lookups, and connection pool wait times.
/// Consumers subscribe via <c>AddMeter("Servus.Akka")</c> in the OTel SDK.
/// </summary>
public static class ServusMetrics
{
    public const string MeterName = "Servus";

    private static readonly string Version =
        typeof(ServusMetrics).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? typeof(ServusMetrics).Assembly.GetName().Version?.ToString()
        ?? "0.0.0";

    public static Meter Meter { get; } = new(MeterName, Version);

    /// <summary>
    /// Number of open connections.
    /// Tags: <c>http.connection.state</c> (<c>"active"</c> or <c>"idle"</c>),
    /// <c>server.address</c>, <c>server.port</c>.
    /// </summary>
    public static UpDownCounter<long> OpenConnections { get; } =
        Meter.CreateUpDownCounter<long>(
            "http.client.open_connections",
            unit: "{connection}",
            description: "Number of currently open transport connections");

    /// <summary>
    /// Connection lifetime in seconds.
    /// Tags: <c>server.address</c>, <c>server.port</c>.
    /// </summary>
    public static Histogram<double> ConnectionDuration { get; } =
        Meter.CreateHistogram<double>(
            "http.client.connection.duration",
            unit: "s",
            description: "Duration of transport connections in seconds");

    /// <summary>
    /// Time spent waiting for an available connection from the pool, in seconds.
    /// Tags: <c>server.address</c>, <c>server.port</c>.
    /// </summary>
    public static Histogram<double> RequestTimeInQueue { get; } =
        Meter.CreateHistogram<double>(
            "http.client.request.time_in_queue",
            unit: "s",
            description: "Time spent waiting for a connection from the pool");

    /// <summary>
    /// Duration of DNS lookups, in seconds.
    /// Tags: <c>dns.question.name</c>, <c>error.type</c> (if failed).
    /// </summary>
    public static Histogram<double> DnsLookupDuration { get; } =
        Meter.CreateHistogram<double>(
            "dns.lookup.duration",
            unit: "s",
            description: "Duration of DNS lookups");
}