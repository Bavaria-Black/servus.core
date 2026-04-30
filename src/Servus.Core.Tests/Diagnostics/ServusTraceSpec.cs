using System.Diagnostics;
using Servus.Core.Diagnostics;
using Xunit;
using TraceLevel = Servus.Core.Diagnostics.TraceLevel;

namespace Servus.Core.Tests.Diagnostics;

[Collection("OTEL")]
public sealed class ServusTraceSpec : IDisposable
{
    private sealed class MockListener : IServusTraceListener
    {
        public List<TraceEvent> Events { get; } = [];
        public bool IsEnabled(TraceLevel level, string category) => true;
        public void Write(in TraceEvent evt) => Events.Add(evt);
    }

    private readonly MockListener _mock = new();

    public ServusTraceSpec()
    {
        Servus.Tracing.Disable();
    }

    public void Dispose()
    {
        Servus.Tracing.Disable();
    }

    [Fact(Timeout = 5000)]
    public void ServusTraceEvent_FormatMessage_should_return_template_when_no_args()
    {
        var evt = new TraceEvent(
            Stopwatch.GetTimestamp(), TraceLevel.Debug, "Connection",
            "Test", 0, "Hello world");

        Assert.Equal("Hello world", evt.FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void ServusTraceEvent_FormatMessage_should_format_args_correctly()
    {
        var evt = new TraceEvent(
            Stopwatch.GetTimestamp(), TraceLevel.Debug, "Pool",
            "Test", 0, "Key={0} Value={1}", "host", 443);

        Assert.Equal("Key=host Value=443", evt.FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void ShouldTrace_should_return_false_when_disabled()
    {
        Assert.False(Servus.Tracing.ShouldTrace("Connection", TraceLevel.Debug));
        Assert.False(Servus.Tracing.ShouldTrace("Pool", TraceLevel.Error));
    }

    [Fact(Timeout = 5000)]
    public void ShouldTrace_should_return_true_when_configured()
    {
        Servus.Tracing.Configure(_mock);

        Assert.True(Servus.Tracing.ShouldTrace("Connection", TraceLevel.Debug));
        Assert.True(Servus.Tracing.ShouldTrace("Pool", TraceLevel.Warning));
    }

    [Fact(Timeout = 5000)]
    public void ShouldTrace_should_respect_category_filter()
    {
        Servus.Tracing.Configure(_mock, TraceLevel.Trace, x => x == "Connection");

        Assert.True(Servus.Tracing.ShouldTrace("Connection", TraceLevel.Debug));
        Assert.False(Servus.Tracing.ShouldTrace("Dns", TraceLevel.Debug));
        Assert.False(Servus.Tracing.ShouldTrace("Pool", TraceLevel.Debug));
    }

    [Fact(Timeout = 5000)]
    public void ShouldTrace_should_respect_minimum_level()
    {
        Servus.Tracing.Configure(_mock, TraceLevel.Warning);

        Assert.False(Servus.Tracing.ShouldTrace("Connection", TraceLevel.Debug));
        Assert.True(Servus.Tracing.ShouldTrace("Connection", TraceLevel.Warning));
        Assert.True(Servus.Tracing.ShouldTrace("Connection", TraceLevel.Error));
    }

    [Fact(Timeout = 5000)]
    public void Connection_Debug_should_emit_event_when_configured()
    {
        Servus.Tracing.Configure(_mock);
        var connection = Servus.Tracing.For("Connection");
        connection.Debug(this, "tcp connected to {0}:{1}", "localhost", 443);

        Assert.Single(_mock.Events);
        var evt = _mock.Events[0];
        Assert.Equal(TraceLevel.Debug, evt.Level);
        Assert.Equal("Connection", evt.Category);
        Assert.Equal(GetType().Name, evt.SourceType);
        Assert.Equal("tcp connected to localhost:443", evt.FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void Dns_Warning_should_emit_event_when_configured()
    {
        Servus.Tracing.Configure(_mock);
        var dns = Servus.Tracing.For("Dns");
        dns.Warning(this, "DNS '{0}' failed: {1}", "badhost", "NXDOMAIN");

        Assert.Single(_mock.Events);
        var evt = _mock.Events[0];
        Assert.Equal(TraceLevel.Warning, evt.Level);
        Assert.Equal("Dns", evt.Category);
        Assert.Equal("DNS 'badhost' failed: NXDOMAIN", evt.FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void Tls_Debug_should_not_emit_when_category_not_enabled()
    {
        Servus.Tracing.Configure(_mock, TraceLevel.Trace, x => x == "Connection");
        var tls = Servus.Tracing.For("Tls");
        tls.Debug(this, "TLS handshake starting");

        Assert.Empty(_mock.Events);
    }

    [Fact(Timeout = 5000)]
    public void Pool_Debug_should_not_emit_when_disabled()
    {
        var pool = Servus.Tracing.For("Pool");
        pool.Debug(this, "Establishing connection");

        Assert.Empty(_mock.Events);
    }

    [Fact(Timeout = 5000)]
    public void Disable_should_stop_subsequent_trace_calls()
    {
        Servus.Tracing.Configure(_mock);

        var connection = Servus.Tracing.For("Connection");
        connection.Debug(this, "first event");
        Servus.Tracing.Disable();
        connection.Debug(this, "after disable");

        Assert.Single(_mock.Events);
        Assert.Equal("first event", _mock.Events[0].FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void Connection_no_args_overload_should_emit_plain_message()
    {
        Servus.Tracing.Configure(_mock);
        var connection = Servus.Tracing.For("Connection");
        connection.Debug(this, "connection disposed");

        Assert.Single(_mock.Events);
        Assert.Equal("connection disposed", _mock.Events[0].FormatMessage());
    }

    [Fact(Timeout = 5000)]
    public void SourceType_should_resolve_correct_type_name_for_different_sources()
    {
        Servus.Tracing.Configure(_mock);
        var channel = Servus.Tracing.For("Test");

        channel.Debug(this, "from test");
        channel.Debug(_mock, "from mock");
        channel.Debug("hello", "from string");

        Assert.Equal(3, _mock.Events.Count);
        Assert.Equal(nameof(ServusTraceSpec), _mock.Events[0].SourceType);
        Assert.Equal(nameof(MockListener), _mock.Events[1].SourceType);
        Assert.Equal(nameof(String), _mock.Events[2].SourceType);
    }
}
