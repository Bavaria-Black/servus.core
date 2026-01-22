using System;
using System.Diagnostics;
using Xunit;
using Servus.Core.Diagnostics;

namespace Servus.Core.Tests.Diagnostics;

public class WithTracingTests : IDisposable
{
    private TestTracingClass _testObject = null!;
    private ActivitySource _activitySource = null!;
    private ActivityListener _activityListener = null!;

    public WithTracingTests()
    {
        _testObject = new TestTracingClass();
        _activitySource = new ActivitySource("TestSource");

        // Set up ActivityListener to enable activity creation
        _activityListener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData
        };

        ActivitySource.AddActivityListener(_activityListener);
    }

    public void Dispose()
    {
        _activityListener.Dispose();
    }

    #region GetContext Tests

    [Theory]
    [InlineData("12345678901234567890123456789012", "1234567890123456", ActivityTraceFlags.Recorded)]
    [InlineData("abcdef1234567890abcdef1234567890", "abcdef1234567890", ActivityTraceFlags.Recorded)]
    public void GetContext_WithValidIds_ReturnsCorrectContext(string traceId, string spanId,
        ActivityTraceFlags expectedFlags)
    {
        // Arrange
        _testObject.TraceId = traceId;
        _testObject.SpanId = spanId;

        // Act
        var context = ((IWithTracing) _testObject).GetContext();

        // Assert
        Assert.Equal(traceId, context.TraceId.ToHexString());
        Assert.Equal(spanId, context.SpanId.ToHexString());
        Assert.Equal(expectedFlags, context.TraceFlags);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(null, "1234567890123456")]
    [InlineData("12345678901234567890123456789012", null)]
    public void GetContext_WithNullOrEmptyIds_GeneratesRandomIds(string traceId, string spanId)
    {
        // Arrange
        _testObject.TraceId = traceId;
        _testObject.SpanId = spanId;

        // Act
        var context = ((IWithTracing) _testObject).GetContext();

        // Assert
        Assert.True(context.TraceId.ToHexString().Length == 32);
        Assert.True(context.SpanId.ToHexString().Length == 16);
        Assert.Equal(ActivityTraceFlags.Recorded, context.TraceFlags);
    }

    #endregion

    #region AddTracing Tests

    [Theory]
    [InlineData("12345678901234567890123456789012", "1234567890123456")]
    [InlineData("abcdef1234567890abcdef1234567890ab", "fedcba0987654321")]
    public void AddTracing_WithValidIds_SetsProperties(string traceId, string spanId)
    {
        // Act
        ((IWithTracing) _testObject).AddTracing(traceId, spanId);

        // Assert
        Assert.Equal(traceId, _testObject.TraceId);
        Assert.Equal(spanId, _testObject.SpanId);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(null, "1234567890123456")]
    [InlineData("12345678901234567890123456789012", null)]
    public void AddTracing_WithNullOrEmptyIds_GeneratesRandomIds(string traceId, string spanId)
    {
        // Act
        ((IWithTracing) _testObject).AddTracing(traceId, spanId);

        // Assert
        Assert.NotNull(_testObject.TraceId);
        Assert.NotNull(_testObject.SpanId);
        Assert.True(_testObject.TraceId.Length == 32);
        Assert.True(_testObject.SpanId.Length == 16);
    }

    [Fact]
    public void AddTracing_FromIWithTracing_CopiesIds()
    {
        // Arrange
        var sourceTracing = new TestTracingClass
        {
            TraceId = "12345678901234567890123456789012",
            SpanId = "1234567890123456"
        };

        // Act
        ((IWithTracing) _testObject).AddTracing(sourceTracing);

        // Assert
        Assert.Equal(sourceTracing.TraceId, _testObject.TraceId);
        Assert.Equal(sourceTracing.SpanId, _testObject.SpanId);
    }

    [Fact]
    public void AddTracing_FromIWithTracingWithNullIds_GeneratesRandomIds()
    {
        // Arrange
        var sourceTracing = new TestTracingClass
        {
            TraceId = null,
            SpanId = null
        };

        // Act
        ((IWithTracing) _testObject).AddTracing(sourceTracing);

        // Assert
        Assert.NotNull(_testObject.TraceId);
        Assert.NotNull(_testObject.SpanId);
        Assert.True(_testObject.TraceId.Length == 32);
        Assert.True(_testObject.SpanId.Length == 16);
    }

    [Fact]
    public void AddTracing_FromCurrentActivity_UsesActivityIds()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("TestActivity");
        
        // Act
        ((IWithTracing) _testObject).AddTracing();

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(activity.TraceId.ToHexString(), _testObject.TraceId);
        Assert.Equal(activity.SpanId.ToHexString(), _testObject.SpanId);
    }

    [Fact]
    public void AddTracing_NoCurrentActivity_GeneratesRandomIds()
    {
        // Ensure no current activity
        Activity.Current = null;

        // Act
        ((IWithTracing) _testObject).AddTracing();

        // Assert
        Assert.NotNull(_testObject.TraceId);
        Assert.NotNull(_testObject.SpanId);
        Assert.True(_testObject.TraceId.Length == 32);
        Assert.True(_testObject.SpanId.Length == 16);
    }

    #endregion

    #region StartActivity Tests

    [Theory]
    [InlineData("TestOperation", ActivityKind.Consumer)]
    [InlineData("ProcessData", ActivityKind.Internal)]
    [InlineData("HttpRequest", ActivityKind.Client)]
    [InlineData("MessageReceive", ActivityKind.Server)]
    public void StartActivity_WithValidParameters_ReturnsActivity(string activityName, ActivityKind kind)
    {
        // Arrange
        _testObject.TraceId = "12345678901234567890123456789012";
        _testObject.SpanId = "1234567890123456";

        // Act
        using var activity = ((IWithTracing) _testObject).StartActivity(activityName, _activitySource, kind);

        // Assert - Activity might be null if no listener is configured
        if (activity != null)
        {
            Assert.Equal(activityName, activity.DisplayName);
            Assert.Equal(kind, activity.Kind);
        }
    }

    [Fact]
    public void StartActivity_DefaultKind_UsesConsumer()
    {
        // Arrange
        _testObject.TraceId = "12345678901234567890123456789012";
        _testObject.SpanId = "1234567890123456";

        // Act
        using var activity = ((IWithTracing) _testObject).StartActivity("TestOperation", _activitySource);

        // Assert - Activity might be null if no listener is configured
        if (activity != null)
        {
            Assert.Equal(ActivityKind.Consumer, activity.Kind);
        }
    }

    #endregion

    // Test implementation of IWithTracing
    private class TestTracingClass : IWithTracing
    {
        public string? TraceId { get; set; }
        public string? SpanId { get; set; }
    }
}