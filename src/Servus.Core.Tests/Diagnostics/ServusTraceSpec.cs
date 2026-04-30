using System.Diagnostics;
using Servus.Core.Diagnostics;
using Xunit;

namespace Servus.Core.Tests.Diagnostics;

[Collection("OTEL")]
public sealed class ServusTraceSpec : IDisposable
{
    private sealed class MockListener : IServusTraceListener
    {
        public List<ServusTraceEvent> Events { get; } = [];
        public bool IsEnabled(ServusTraceLevel level, string category) => true;
        public void Write(in ServusTraceEvent evt) => Events.Add(evt);
    }

    private readonly MockListener _mock = new();

    public ServusTraceSpec()
    {
        ServusTrace.Disable();
    }

    public void Dispose()
    {
        ServusTrace.Disable();
    }

    [Fact(Timeout = 5000)]
    public void ServusTraceEvent_FormatMessage_should_return_template_when_no_args()
    {
        var evt = new ServusTraceEvent(
            Stopwatch.GetTimestamp(), ServusTraceLevel.Debug, "Connection",
            "Test", 0, "Hello world");

        Assert.Equal("Hello world", evt.FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void ServusTraceEvent_FormatMessage_should_format_args_correctly()
    {
        var evt = new ServusTraceEvent(
            Stopwatch.GetTimestamp(), ServusTraceLevel.Debug, "Pool",
            "Test", 0, "Key={0} Value={1}", "host", 443);

        Assert.Equal("Key=host Value=443", evt.FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void ShouldTrace_should_return_false_when_disabled()
    {
        Assert.False(ServusTrace.ShouldTrace("Connection", ServusTraceLevel.Debug));
        Assert.False(ServusTrace.ShouldTrace("Pool", ServusTraceLevel.Error));
    }

    [Fact(Timeout = 5000)]
    public void ShouldTrace_should_return_true_when_configured()
    {
        ServusTrace.Configure(_mock);

        Assert.True(ServusTrace.ShouldTrace("Connection", ServusTraceLevel.Debug));
        Assert.True(ServusTrace.ShouldTrace("Pool", ServusTraceLevel.Warning));
    }

    [Fact(Timeout = 5000)]
    public void ShouldTrace_should_respect_category_filter()
    {
        ServusTrace.Configure(_mock, ServusTraceLevel.Trace, x => x == "Connection");

        Assert.True(ServusTrace.ShouldTrace("Connection", ServusTraceLevel.Debug));
        Assert.False(ServusTrace.ShouldTrace("Dns", ServusTraceLevel.Debug));
        Assert.False(ServusTrace.ShouldTrace("Pool", ServusTraceLevel.Debug));
    }

    [Fact(Timeout = 5000)]
    public void ShouldTrace_should_respect_minimum_level()
    {
        ServusTrace.Configure(_mock, ServusTraceLevel.Warning);

        Assert.False(ServusTrace.ShouldTrace("Connection", ServusTraceLevel.Debug));
        Assert.True(ServusTrace.ShouldTrace("Connection", ServusTraceLevel.Warning));
        Assert.True(ServusTrace.ShouldTrace("Connection", ServusTraceLevel.Error));
    }

    [Fact(Timeout = 5000)]
    public void Connection_Debug_should_emit_event_when_configured()
    {
        ServusTrace.Configure(_mock);
        var connection = ServusTrace.For("Connection");
        connection.Debug(this, "tcp connected to {0}:{1}", "localhost", 443);

        Assert.Single(_mock.Events);
        var evt = _mock.Events[0];
        Assert.Equal(ServusTraceLevel.Debug, evt.Level);
        Assert.Equal("Connection", evt.Category);
        Assert.Equal(GetType().Name, evt.SourceType);
        Assert.Equal("tcp connected to localhost:443", evt.FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void Dns_Warning_should_emit_event_when_configured()
    {
        ServusTrace.Configure(_mock);
        var dns = ServusTrace.For("Dns");
        dns.Warning(this, "DNS '{0}' failed: {1}", "badhost", "NXDOMAIN");

        Assert.Single(_mock.Events);
        var evt = _mock.Events[0];
        Assert.Equal(ServusTraceLevel.Warning, evt.Level);
        Assert.Equal("Dns", evt.Category);
        Assert.Equal("DNS 'badhost' failed: NXDOMAIN", evt.FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void Tls_Debug_should_not_emit_when_category_not_enabled()
    {
        ServusTrace.Configure(_mock, ServusTraceLevel.Trace, x => x == "Connection");
        var tls = ServusTrace.For("Tls");
        tls.Debug(this, "TLS handshake starting");

        Assert.Empty(_mock.Events);
    }

    [Fact(Timeout = 5000)]
    public void Pool_Debug_should_not_emit_when_disabled()
    {
        var pool = ServusTrace.For("Pool");
        pool.Debug(this, "Establishing connection");

        Assert.Empty(_mock.Events);
    }

    [Fact(Timeout = 5000)]
    public void Disable_should_stop_subsequent_trace_calls()
    {
        ServusTrace.Configure(_mock);

        var connection = ServusTrace.For("Connection");
        connection.Debug(this, "first event");
        ServusTrace.Disable();
        connection.Debug(this, "after disable");

        Assert.Single(_mock.Events);
        Assert.Equal("first event", _mock.Events[0].FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void Connection_no_args_overload_should_emit_plain_message()
    {
        ServusTrace.Configure(_mock);
        var connection = ServusTrace.For("Connection");
        connection.Debug(this, "connection disposed");

        Assert.Single(_mock.Events);
        Assert.Equal("connection disposed", _mock.Events[0].FormatMessage());
    }
}
