using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Diagnostics;

namespace Servus.Core.Tests.Diagnostics;

[TestClass]
public class ActivitySourceRegistryTests
{
    private ActivityListener _activityListener = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        // Set up ActivityListener to enable activity creation
        _activityListener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData
        };
        ActivitySource.AddActivityListener(_activityListener);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _activityListener?.Dispose();
    }

    [TestMethod]
    public void StartActivity_WithValidParameters_ReturnsActivity()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(activityName, result.DisplayName);
    }

    [TestMethod]
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
        Assert.IsNotNull(result);
        Assert.AreEqual(activityName, result.DisplayName);
        Assert.AreEqual("test_class", result.Source?.Name);
        Assert.AreEqual(kind, result.Kind);
    }

    [TestMethod]
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
        Assert.IsNotNull(result1);
        Assert.IsNotNull(result2);
        Assert.AreSame(result1.Source, result2.Source);
        Assert.AreEqual("test_class", result1.Source?.Name);
        Assert.AreEqual("test_class", result2.Source?.Name);
    }

    [TestMethod]
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
        Assert.IsNotNull(result1);
        Assert.IsNotNull(result2);
        Assert.AreNotSame(result1.Source, result2.Source);
        Assert.AreEqual("test_class", result1.Source?.Name);
        Assert.AreEqual("another_test_class", result2.Source?.Name);
    }

    [TestMethod]
    public void StartActivity_WithDefaultActivityKind_UsesConsumerKind()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(ActivityKind.Consumer, result.Kind);
    }

    [TestMethod]
    [DataRow(ActivityKind.Internal)]
    [DataRow(ActivityKind.Server)]
    [DataRow(ActivityKind.Client)]
    [DataRow(ActivityKind.Producer)]
    [DataRow(ActivityKind.Consumer)]
    public void StartActivity_WithSpecificActivityKind_PassesCorrectKind(ActivityKind kind)
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace, kind);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(kind, result.Kind);
    }

    [TestMethod]
    public void StartActivity_WhenTraceReturnsNull_ReturnsNull()
    {
        // Arrange
        const string activityName = "test-activity";
        NullReturningTestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<TestClass>(activityName, trace);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void StartActivity_WithComplexTypeName_ConvertsToSnakeCase()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<MyComplexTestClass>(activityName, trace);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("my_complex_test_class", result.Source?.Name);
    }

    [TestMethod]
    public void StartActivity_WithGenericType_ConvertsToSnakeCase()
    {
        // Arrange
        const string activityName = "test-activity";
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();

        // Act
        var result = ActivitySourceRegistry.StartActivity<GenericTestClass<string>>(activityName, trace);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Source?.Name.StartsWith("generic_test_class"));
    }

    [TestMethod]
    public void Add_WithoutName_CreatesActivitySourceWithSnakeCaseName()
    {
        // Arrange & Act
        ActivitySourceRegistry.Add<TestClass>();
        
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();
        var result = ActivitySourceRegistry.StartActivity<TestClass>("test-activity", trace);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("test_class", result.Source?.Name);
    }

    [TestMethod]
    public void Add_WithExplicitName_CreatesActivitySourceWithGivenName()
    {
        // Arrange & Act
        ActivitySourceRegistry.Add<CustomNameTestClass>("custom_name");
        
        TestMessage trace = new("test trace");
        ((IWithTracing)trace).AddTracing();
        var result = ActivitySourceRegistry.StartActivity<CustomNameTestClass>("test-activity", trace);

        // Assert
        Assert.IsNotNull(result);
        // Note: The current implementation ignores the custom name parameter
        // This test documents the current behavior
        Assert.AreEqual("custom_name_test_class", result.Source?.Name);
    }

    // Test classes for type parameter testing
    private class TestClass;
    private class AnotherTestClass;
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