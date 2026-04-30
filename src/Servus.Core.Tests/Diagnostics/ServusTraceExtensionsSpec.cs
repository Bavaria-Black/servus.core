using Microsoft.Extensions.DependencyInjection;
using Servus.Core.Diagnostics;
using Xunit;

namespace Servus.Core.Tests.Diagnostics;

[Collection("OTEL")]
public sealed class ServusTraceExtensionsSpec : IDisposable
{
    private sealed class MockListener : IServusTraceListener
    {
        public bool IsEnabled(TraceLevel level, string category) => true;
        public void Write(in TraceEvent evt) { }
    }

    public void Dispose()
    {
        ServusTrace.Disable();
    }

    [Fact(Timeout = 5000)]
    public void AddServusLoggerTracing_should_register_IServusTraceListener()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddServusLoggerTracing();

        var provider = services.BuildServiceProvider();
        var listener = provider.GetService<IServusTraceListener>();

        Assert.NotNull(listener);
        Assert.IsType<TraceLogger>(listener);
    }

    [Fact(Timeout = 5000)]
    public void AddServusLoggerTracing_should_configure_ServusTrace_on_resolve()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddServusLoggerTracing(TraceLevel.Debug, "Connection");

        var provider = services.BuildServiceProvider();

        Assert.False(ServusTrace.ShouldTrace("Connection", TraceLevel.Debug));
        _ = provider.GetRequiredService<IServusTraceListener>();

        Assert.True(ServusTrace.ShouldTrace("Connection", TraceLevel.Debug));
    }

    [Fact(Timeout = 5000)]
    public void AddServusTraceListener_should_register_custom_listener_and_configure_ServusTrace()
    {
        var listener = new MockListener();
        var services = new ServiceCollection();
        services.AddServusTraceListener(listener);

        Assert.True(ServusTrace.ShouldTrace("Connection", TraceLevel.Debug));
    }

    [Fact(Timeout = 5000)]
    public void AddServusTraceListener_should_throw_when_listener_is_null()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() =>
            services.AddServusTraceListener(null!));
    }

    [Fact(Timeout = 5000)]
    public void AddServusLoggerTracing_should_respect_category_filter()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddServusLoggerTracing(TraceLevel.Debug, "Connection");

        var provider = services.BuildServiceProvider();
        _ = provider.GetRequiredService<IServusTraceListener>();

        Assert.True(ServusTrace.ShouldTrace("Connection", TraceLevel.Debug));
        Assert.False(ServusTrace.ShouldTrace("Dns", TraceLevel.Debug));
    }
}
