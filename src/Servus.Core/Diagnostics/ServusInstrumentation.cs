using System.Diagnostics;
using System.Reflection;

namespace Servus.Core.Diagnostics;

/// <summary>
/// OpenTelemetry tracing for the Servus.Akka transport layer.
/// Emits spans for connection establishment, DNS resolution, socket connect,
/// TLS handshake, and connection pool wait times.
/// Consumers subscribe via <c>AddSource("Servus")</c> in the OTel SDK.
/// </summary>
public static class ServusInstrumentation
{
    public const string SourceName = "Servus";

    private static readonly string Version =
        typeof(ServusInstrumentation).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? typeof(ServusInstrumentation).Assembly.GetName().Version?.ToString()
        ?? "0.0.0";

    public static ActivitySource Source { get; } = new(SourceName, Version);

    public static Activity? StartConnect(Uri uri)
    {
        if (!Source.HasListeners())
        {
            return null;
        }

        var activity = Source.StartActivity($"{SourceName}.Connect", ActivityKind.Client);
        if (activity is null)
        {
            return null;
        }

        activity.SetTag("server.address", uri.Host);
        activity.SetTag("server.port", uri.Port);
        activity.SetTag("url.scheme", uri.Scheme);

        return activity;
    }

    public static Activity? StartDnsLookup(string hostname)
    {
        if (!Source.HasListeners())
        {
            return null;
        }

        var activity = Source.StartActivity($"{SourceName}.DnsLookup", ActivityKind.Client);
        if (activity is null)
        {
            return null;
        }

        activity.SetTag("dns.question.name", hostname);

        return activity;
    }

    /// <param name="address">The peer IP address (e.g., "93.184.216.34").</param>
    /// <param name="port">The peer port number.</param>
    /// <param name="transport">The transport protocol: <c>"tcp"</c>, <c>"udp"</c>, or <c>"unix"</c>.</param>
    /// <param name="networkType">The network type: <c>"ipv4"</c> or <c>"ipv6"</c>. Null for non-IP transports.</param>
    public static Activity? StartSocketConnect(string address, int port,
        string transport = "tcp", string? networkType = null)
    {
        if (!Source.HasListeners())
        {
            return null;
        }

        var activity = Source.StartActivity($"{SourceName}.SocketConnect", ActivityKind.Client);
        if (activity is null)
        {
            return null;
        }

        activity.SetTag("network.peer.address", address);
        activity.SetTag("network.peer.port", port);
        activity.SetTag("network.transport", transport);
        if (networkType is not null)
        {
            activity.SetTag("network.type", networkType);
        }

        return activity;
    }

    public static Activity? StartTlsHandshake(string host)
    {
        if (!Source.HasListeners())
        {
            return null;
        }

        var activity = Source.StartActivity($"{SourceName}.TlsHandshake", ActivityKind.Client);
        if (activity is null)
        {
            return null;
        }

        activity.SetTag("server.address", host);

        return activity;
    }

    public static Activity? StartWaitForConnection(string address, int port)
    {
        if (!Source.HasListeners())
        {
            return null;
        }

        var activity = Source.StartActivity($"{SourceName}.WaitForConnection", ActivityKind.Client);
        if (activity is null)
        {
            return null;
        }

        activity.SetTag("server.address", address);
        activity.SetTag("server.port", port);

        return activity;
    }

    public static void SetTlsInfo(Activity activity, string protocolName, string protocolVersion)
    {
        activity.SetTag("tls.protocol.name", protocolName);
        activity.SetTag("tls.protocol.version", protocolVersion);
    }

    public static void SetDnsAnswers(Activity activity, string[] answers)
    {
        activity.SetTag("dns.answers", answers);
    }

    public static void SetNetworkPeerAddress(Activity activity, string address)
    {
        activity.SetTag("network.peer.address", address);
    }

    public static void SetError(Activity activity, Exception exception)
    {
        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.SetTag("error.type", exception.GetType().FullName);
    }
}