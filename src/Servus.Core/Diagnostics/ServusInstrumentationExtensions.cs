using System.Diagnostics;

namespace Servus.Core.Diagnostics;

/// <summary>
/// Built-in HTTP-transport activity starters and semantic tag helpers
/// for <see cref="ServusInstrumentation"/> and <see cref="Activity"/>.
/// </summary>
public static class ServusInstrumentationExtensions
{
    public static Activity? StartConnect(this ServusInstrumentation instr, Uri uri)
    {
        if (!instr.Source.HasListeners()) return null;
        var activity = instr.Source.StartActivity("connect", ActivityKind.Client);
        if (activity is null) return null;

        activity.SetTag("server.address", uri.Host);
        activity.SetTag("server.port", uri.Port);
        activity.SetTag("url.scheme", uri.Scheme);
        return activity;
    }

    public static Activity? StartDnsLookup(this ServusInstrumentation instr, string hostname)
    {
        if (!instr.Source.HasListeners()) return null;
        var activity = instr.Source.StartActivity("dns.lookup", ActivityKind.Client);
        if (activity is null) return null;

        activity.SetTag("dns.question.name", hostname);
        return activity;
    }

    /// <param name="instr">The instrumentation channel.</param>
    /// <param name="address">The peer IP address (e.g., "93.184.216.34").</param>
    /// <param name="port">The peer port number.</param>
    /// <param name="transport">The transport protocol: <c>"tcp"</c>, <c>"udp"</c>, or <c>"unix"</c>.</param>
    /// <param name="networkType">The network type: <c>"ipv4"</c> or <c>"ipv6"</c>. Null for non-IP transports.</param>
    public static Activity? StartSocketConnect(this ServusInstrumentation instr, string address, int port,
        string transport = "tcp", string? networkType = null)
    {
        if (!instr.Source.HasListeners()) return null;
        var activity = instr.Source.StartActivity("socket.connect", ActivityKind.Client);
        if (activity is null) return null;

        activity.SetTag("network.peer.address", address);
        activity.SetTag("network.peer.port", port);
        activity.SetTag("network.transport", transport);
        if (networkType is not null)
            activity.SetTag("network.type", networkType);
        return activity;
    }

    public static Activity? StartTlsHandshake(this ServusInstrumentation instr, string host)
    {
        if (!instr.Source.HasListeners()) return null;
        var activity = instr.Source.StartActivity("tls.handshake", ActivityKind.Client);
        if (activity is null) return null;

        activity.SetTag("server.address", host);
        return activity;
    }

    public static Activity? StartWaitForConnection(this ServusInstrumentation instr, string address, int port)
    {
        if (!instr.Source.HasListeners()) return null;
        var activity = instr.Source.StartActivity("connection.wait", ActivityKind.Client);
        if (activity is null) return null;

        activity.SetTag("server.address", address);
        activity.SetTag("server.port", port);
        return activity;
    }

    public static void SetTlsInfo(this Activity activity, string protocolName, string protocolVersion)
    {
        activity.SetTag("tls.protocol.name", protocolName);
        activity.SetTag("tls.protocol.version", protocolVersion);
    }

    public static void SetDnsAnswers(this Activity activity, string[] answers)
        => activity.SetTag("dns.answers", answers);

    public static void SetNetworkPeerAddress(this Activity activity, string address)
        => activity.SetTag("network.peer.address", address);

    public static void SetError(this Activity activity, Exception exception)
    {
        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.SetTag("error.type", exception.GetType().FullName);
    }
}