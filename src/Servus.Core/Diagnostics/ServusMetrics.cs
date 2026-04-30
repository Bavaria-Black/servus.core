using System.Diagnostics.Metrics;

namespace Servus.Core.Diagnostics;

/// <summary>
/// A named metrics channel wrapping a <see cref="Meter"/>.
/// Use <see cref="MeterName"/> for the shared Servus meter, or create named instances per module.
/// Extend with instrument factories via extension methods on this type.
/// Consumers subscribe via <c>AddMeter("Servus")</c> (or your module name) in the OTel SDK.
/// </summary>
/// <example>
/// <code>
/// // Shared default meter
/// private static readonly UpDownCounter&lt;long&gt; _conns = ServusMetrics.Instance.AddOpenConnections();
///
/// // Per-module meter
/// private static readonly ServusMetrics _metrics = new("MyModule");
/// private static readonly UpDownCounter&lt;long&gt; _conns = _metrics.AddOpenConnections();
/// </code>
/// </example>
public class ServusMetrics
{
    public static readonly ServusMetrics Servus = new("Servus");

    public Meter Meter { get; }

    private ServusMetrics(string meterName)
    {
    }
}