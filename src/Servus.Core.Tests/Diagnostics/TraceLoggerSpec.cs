using Microsoft.Extensions.Logging;
using Servus.Core.Diagnostics;
using Xunit;

namespace Servus.Core.Tests.Diagnostics;

[CollectionDefinition("OTEL", DisableParallelization = true)]
public sealed class OTelCollection;

[Collection("OTEL")]
public sealed class TraceLoggerSpec : IDisposable
{
    private sealed class CapturingLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }
    }

    private sealed class CapturingLoggerFactory : ILoggerFactory
    {
        private readonly Dictionary<string, CapturingLogger> _loggers = new();

        public CapturingLogger GetLogger(string name)
        {
            if (!_loggers.TryGetValue(name, out var logger))
            {
                logger = new CapturingLogger();
                _loggers[name] = logger;
            }

            return logger;
        }

        public ILogger CreateLogger(string categoryName) => GetLogger(categoryName);

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }

    private readonly CapturingLoggerFactory _factory = new();

    public void Dispose()
    {
        Servus.Tracing.Disable();
    }

    [Fact(Timeout = 5000)]
    public void IsEnabled_should_return_false_when_level_below_minimum()
    {
        var listener = new TraceLogger(_factory, TraceLevel.Warning);

        
        Servus.Tracing
            .For("leberkas")
            .Debug(this, "blub");
        
        Assert.False(listener.IsEnabled(TraceLevel.Debug, "Connection"));
        Assert.False(listener.IsEnabled(TraceLevel.Info, "Pool"));
    }

    [Fact(Timeout = 5000)]
    public void IsEnabled_should_return_false_when_category_not_enabled()
    {
        var listener =
            new TraceLogger(_factory, TraceLevel.Trace, x => x == "Connection");

        Assert.False(listener.IsEnabled(TraceLevel.Debug, "Dns"));
        Assert.False(listener.IsEnabled(TraceLevel.Error, "Pool"));
    }

    [Fact(Timeout = 5000)]
    public void IsEnabled_should_return_true_when_level_and_category_match()
    {
        var listener = new TraceLogger(_factory);

        Assert.True(listener.IsEnabled(TraceLevel.Debug, "Connection"));
        Assert.True(listener.IsEnabled(TraceLevel.Error, "Tls"));
    }

    [Fact(Timeout = 5000)]
    public void Write_should_route_Connection_event_to_correct_logger()
    {
        var listener = new TraceLogger(_factory);
        var source = new object();
        var evt = new TraceEvent(
            System.Diagnostics.Stopwatch.GetTimestamp(),
            TraceLevel.Debug,
            "Connection",
            source.GetType().Name, source.GetHashCode(), "Connected to {0}:{1}", "localhost", 443);

        listener.Write(in evt);

        var logger = _factory.GetLogger("Servus.Trace.Connection");
        Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Debug, logger.Entries[0].Level);
        Assert.Contains("Connected to localhost:443", logger.Entries[0].Message);
    }

    [Fact(Timeout = 5000)]
    public void Write_should_route_Dns_event_to_Dns_logger()
    {
        var listener = new TraceLogger(_factory);
        var source = new object();
        var evt = new TraceEvent(
            System.Diagnostics.Stopwatch.GetTimestamp(),
            TraceLevel.Warning,
            "Dns",
            source.GetType().Name, source.GetHashCode(), "DNS failed");

        listener.Write(in evt);

        var logger = _factory.GetLogger("Servus.Trace.Dns");
        Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, logger.Entries[0].Level);
    }

    [Fact(Timeout = 5000)]
    public void Write_should_route_Pool_event_to_Pool_logger()
    {
        var listener = new TraceLogger(_factory);
        var source = new object();
        var evt = new TraceEvent(
            System.Diagnostics.Stopwatch.GetTimestamp(),
            TraceLevel.Info,
            "Pool",
            source.GetType().Name, source.GetHashCode(), "Pool evicted");

        listener.Write(in evt);

        var poolLogger = _factory.GetLogger("Servus.Trace.Pool");
        Assert.Single(poolLogger.Entries);
    }

    [Fact(Timeout = 5000)]
    public void Write_should_map_ServusTraceLevel_to_LogLevel_correctly()
    {
        var listener = new TraceLogger(_factory, TraceLevel.Trace);
        var source = new object();

        void Send(TraceLevel level) =>
            listener.Write(new TraceEvent(
                System.Diagnostics.Stopwatch.GetTimestamp(), level, "Tls",
                source.GetType().Name, source.GetHashCode(), "msg"));

        Send(TraceLevel.Trace);
        Send(TraceLevel.Debug);
        Send(TraceLevel.Info);
        Send(TraceLevel.Warning);
        Send(TraceLevel.Error);

        var logger = _factory.GetLogger("Servus.Trace.Tls");
        Assert.Equal(5, logger.Entries.Count);
        Assert.Equal(LogLevel.Trace, logger.Entries[0].Level);
        Assert.Equal(LogLevel.Debug, logger.Entries[1].Level);
        Assert.Equal(LogLevel.Information, logger.Entries[2].Level);
        Assert.Equal(LogLevel.Warning, logger.Entries[3].Level);
        Assert.Equal(LogLevel.Error, logger.Entries[4].Level);
    }
}
