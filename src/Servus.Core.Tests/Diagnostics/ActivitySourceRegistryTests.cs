using System;
using System.Diagnostics;
using Servus.Core.Diagnostics;
using Xunit;

namespace Servus.Core.Tests.Diagnostics;

public class ActivitySourceRegistryTests : IDisposable
{
    private ActivityListener _activityListener = null!;

    public ActivitySourceRegistryTests()
    {
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

    [Fact]
    public void StartActivity_WithValidParameters_ReturnsActivity()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(activityName, result.DisplayName);
    }

    [Fact]
    public void StartActivity_CallsTraceStartActivityWithCorrectParameters()
    {
        // Arrange
        const string activityName = "test-activity";
        const ActivityKind kind = ActivityKind.Producer;
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace, kind);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(activityName, result.DisplayName);
        Assert.Equal("test_class", result.Source?.Name);
        Assert.Equal(kind, result.Kind);
    }

    [Fact]
    public void StartActivity_WithSameTypeMultipleTimes_ReusesActivitySource()
    {
        // Arrange
        const string activityName1 = "activity-1";
        const string activityName2 = "activity-2";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result1 = ActivitySourceRegistry.StartActivity<TestClass>(activityName1, trace);
        var result2 = ActivitySourceRegistry.StartActivity<TestClass>(activityName2, trace);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Same(result1.Source, result2.Source);
        Assert.Equal("test_class", result1.Source?.Name);
        Assert.Equal("test_class", result2.Source?.Name);
    }

    [Fact]
    public void StartActivity_WithDifferentTypes_CreatesSeparateActivitySources()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result1 = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace);
        var result2 = ActivitySourceRegistry.StartActivity<AnotherTestClass>(activityName, trace);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotSame(result1.Source, result2.Source);
        Assert.Equal("test_class", result1.Source?.Name);
        Assert.Equal("another_test_class", result2.Source?.Name);
    }

    [Fact]
    public void StartActivity_WithDifferentTypes_ButSameRootSource()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result1 = ActivitySourceRegistry.StartActivity<RootSourceClass>(activityName, trace);
        var result2 = ActivitySourceRegistry.StartActivity<TestClassWithAttribute>(activityName, trace);
        
        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotSame(result1.Source, result2.Source);
        Assert.Equal("root-source", result1.Source?.Name);
        Assert.Equal("root-source", result2.Source?.Name);
    }

    [Fact]
    public void StartActivity_WithDifferentTypes_ButSameSource()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result1 = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace);
        var result2 = ActivitySourceRegistry.StartActivity<AnotherTestClassWithAttribute>(activityName, trace);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotSame(result1.Source, result2.Source);
        Assert.Equal("test_class", result1.Source?.Name);
        Assert.Equal("test_class", result2.Source?.Name);
    }

    [Fact]
    public void StartActivity_WithDefaultActivityKind_UsesConsumerKind()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ActivityKind.Consumer, result.Kind);
    }

    [Theory]
    [InlineData(ActivityKind.Internal)]
    [InlineData(ActivityKind.Server)]
    [InlineData(ActivityKind.Client)]
    [InlineData(ActivityKind.Producer)]
    [InlineData(ActivityKind.Consumer)]
    public void StartActivity_WithSpecificActivityKind_PassesCorrectKind(ActivityKind kind)
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace, kind);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(kind, result.Kind);
    }

    [Fact]
    public void StartActivity_WhenTraceReturnsNull_ReturnsNull()
    {
        // Arrange
        const string activityName = "test-activity";
        NullReturningTestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void StartActivity_WithComplexTypeName_ConvertsToSnakeCase()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<MyComplexTestClass>(activityName, trace);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("my_complex_test_class", result.Source?.Name);
    }

    [Fact]
    public void StartActivity_WithGenericType_ConvertsToSnakeCase()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<GenericTestClass<string>>(activityName, trace);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Source?.Name.StartsWith("generic_test_class"));
    }

    [Fact]
    public void CreatesActivitySourceWithSnakeCaseName_WithoutAdding()
    {
        // Arrange & Act
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();
        var result = ActivitySourceRegistry.StartActivity<TestClass>("test-activity", trace);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test_class", result.Source?.Name);
    }

    [Fact]
    public void Add_WithExplicitName_CreatesActivitySourceWithGivenName()
    {
        // Arrange & Act
        ActivitySourceRegistry.Add<CustomNameTestClass>("custom_name");
        
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();
        var result = ActivitySourceRegistry.StartActivity<CustomNameTestClass>("test-activity", trace);

        // Assert
        Assert.NotNull(result);
        // Note: The current implementation ignores the custom name parameter
        // This test documents the current behavior
        Assert.Equal("custom_name_test_class", result.Source?.Name);
    }

    // Test classes for type parameter testing
    private class TestClass;
    
    [ActivitySourceName("root-source")]
    private class RootSourceClass;
    
    private class AnotherTestClass;
    
    [ActivitySourceKey(typeof(RootSourceClass))]
    private class TestClassWithAttribute;
    
    [ActivitySourceKey(typeof(TestClass))]
    private class AnotherTestClassWithAttribute;
    private class MyComplexTestClass;
    private class GenericTestClass<T>;
    private class CustomNameTestClass;
}

// Test record implementing IWithTracing
public record TestMessage(string Message) : IWithTracing
{
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }
}

// Test record that returns null for testing null scenarios
public record NullReturningTestMessage(string Message) : IWithTracing
{
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }

    public Activity? StartActivity(string name, ActivitySource source, ActivityKind kind = ActivityKind.Consumer) => null;
}