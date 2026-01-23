using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Diagnostics;

namespace Servus.Core.Tests.Diagnostics;

[TestClass]
public class WithTracingTests
{
    private TestTracingClass _testObject = null!;
    private ActivitySource _activitySource = null!;
    private ActivityListener _activityListener = null!;

    [TestInitialize]
    public void Setup()
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

    [TestCleanup]
    public void Cleanup()
    {
        _activitySource?.Dispose();
    }

    #region GetContext Tests

    [Ignore("Legacy test for obsolete TraceId/SpanId - to be removed")]
    [TestMethod]
    [DataRow("12345678901234567890123456789012", "1234567890123456", ActivityTraceFlags.Recorded)]
    [DataRow("abcdef1234567890abcdef1234567890", "abcdef1234567890", ActivityTraceFlags.Recorded)]
    public void GetContext_WithValidIds_ReturnsCorrectContext(string traceId, string spanId,
        ActivityTraceFlags expectedFlags)
    {
        // Arrange
        _testObject.TraceId = traceId;
        _testObject.SpanId = spanId;

        // Act
        var context = ((IWithTracing)_testObject).GetContext();

        // Assert
        Assert.AreEqual(traceId, context.TraceId.ToHexString());
        Assert.AreEqual(spanId, context.SpanId.ToHexString());
        Assert.AreEqual(expectedFlags, context.TraceFlags);
    }

    [Ignore("Legacy test for obsolete TraceId/SpanId - to be removed")]
    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", "")]
    [DataRow(null, "1234567890123456")]
    [DataRow("12345678901234567890123456789012", null)]
    public void GetContext_WithNullOrEmptyIds_GeneratesRandomIds(string traceId, string spanId)
    {
        // Arrange
        _testObject.TraceId = traceId;
        _testObject.SpanId = spanId;

        // Act
        var context = ((IWithTracing)_testObject).GetContext();

        // Assert
        Assert.IsTrue(context.TraceId.ToHexString().Length == 32);
        Assert.IsTrue(context.SpanId.ToHexString().Length == 16);
        Assert.AreEqual(ActivityTraceFlags.Recorded, context.TraceFlags);
    }

    #endregion

    #region AddTracing Tests

    [Ignore("Legacy test for obsolete TraceId/SpanId - to be removed")]
    [TestMethod]
    [DataRow("12345678901234567890123456789012", "1234567890123456")]
    [DataRow("abcdef1234567890abcdef1234567890ab", "fedcba0987654321")]
    public void AddTracing_WithValidIds_SetsProperties(string traceId, string spanId)
    {
        // Act
        ((IWithTracing)_testObject).AddTracing(traceId, spanId);

        // Assert
        Assert.AreEqual(traceId, _testObject.TraceId);
        Assert.AreEqual(spanId, _testObject.SpanId);
    }

    [Ignore("Legacy test for obsolete TraceId/SpanId - to be removed")]
    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", "")]
    [DataRow(null, "1234567890123456")]
    [DataRow("12345678901234567890123456789012", null)]
    public void AddTracing_WithNullOrEmptyIds_GeneratesRandomIds(string traceId, string spanId)
    {
        // Act
        ((IWithTracing)_testObject).AddTracing(traceId, spanId);

        // Assert
        Assert.IsNotNull(_testObject.TraceId);
        Assert.IsNotNull(_testObject.SpanId);
        Assert.IsTrue(_testObject.TraceId.Length == 32);
        Assert.IsTrue(_testObject.SpanId.Length == 16);
    }

    [Ignore("Legacy test for obsolete TraceId/SpanId - to be removed")]
    [TestMethod]
    public void AddTracing_FromIWithTracing_CopiesIds()
    {
        // Arrange
        var sourceTracing = new TestTracingClass
        {
            TraceId = "12345678901234567890123456789012",
            SpanId = "1234567890123456"
        };

        // Act
        ((IWithTracing)_testObject).AddTracing(sourceTracing);

        // Assert
        Assert.AreEqual(sourceTracing.TraceId, _testObject.TraceId);
        Assert.AreEqual(sourceTracing.SpanId, _testObject.SpanId);
    }

    [Ignore("Legacy test for obsolete TraceId/SpanId - to be removed")]
    [TestMethod]
    public void AddTracing_FromIWithTracingWithNullIds_GeneratesRandomIds()
    {
        // Arrange
        var sourceTracing = new TestTracingClass
        {
            TraceId = null,
            SpanId = null
        };

        // Act
        ((IWithTracing)_testObject).AddTracing(sourceTracing);

        // Assert
        Assert.IsNotNull(_testObject.TraceId);
        Assert.IsNotNull(_testObject.SpanId);
        Assert.IsTrue(_testObject.TraceId.Length == 32);
        Assert.IsTrue(_testObject.SpanId.Length == 16);
    }

    [Ignore("Legacy test for obsolete TraceId/SpanId - to be removed")]
    [TestMethod]
    public void AddTracing_FromCurrentActivity_UsesActivityIds()
    {
        // Arrange
        using var activity = _activitySource.StartActivity("TestActivity");

        // Act
        ((IWithTracing)_testObject).AddTracing();

        // Assert
        Assert.IsNotNull(activity);
        Assert.AreEqual(activity.TraceId.ToHexString(), _testObject.TraceId);
        Assert.AreEqual(activity.SpanId.ToHexString(), _testObject.SpanId);
    }

    [Ignore("Legacy test for obsolete TraceId/SpanId - to be removed")]
    [TestMethod]
    public void AddTracing_NoCurrentActivity_GeneratesRandomIds()
    {
        // Ensure no current activity
        Activity.Current = null;

        // Act
        ((IWithTracing)_testObject).AddTracing();

        // Assert
        Assert.IsNotNull(_testObject.TraceId);
        Assert.IsNotNull(_testObject.SpanId);
        Assert.IsTrue(_testObject.TraceId.Length == 32);
        Assert.IsTrue(_testObject.SpanId.Length == 16);
    }

    #endregion

    #region StartActivity Tests

    [Ignore("Legacy test for obsolete TraceId/SpanId - to be removed")]
    [TestMethod]
    [DataRow("TestOperation", ActivityKind.Consumer)]
    [DataRow("ProcessData", ActivityKind.Internal)]
    [DataRow("HttpRequest", ActivityKind.Client)]
    [DataRow("MessageReceive", ActivityKind.Server)]
    public void StartActivity_WithValidParameters_ReturnsActivity(string activityName, ActivityKind kind)
    {
        // Arrange
        _testObject.TraceId = "12345678901234567890123456789012";
        _testObject.SpanId = "1234567890123456";

        // Act
        using var activity = ((IWithTracing)_testObject).StartActivity(activityName, _activitySource, kind);

        // Assert - Activity might be null if no listener is configured
        if (activity != null)
        {
            Assert.AreEqual(activityName, activity.DisplayName);
            Assert.AreEqual(kind, activity.Kind);
        }
    }

    [Ignore("Legacy test for obsolete TraceId/SpanId - to be removed")]
    [TestMethod]
    public void StartActivity_DefaultKind_UsesConsumer()
    {
        // Arrange
        _testObject.TraceId = "12345678901234567890123456789012";
        _testObject.SpanId = "1234567890123456";

        // Act
        using var activity = ((IWithTracing)_testObject).StartActivity("TestOperation", _activitySource);

        // Assert - Activity might be null if no listener is configured
        if (activity != null)
        {
            Assert.AreEqual(ActivityKind.Consumer, activity.Kind);
        }
    }

    #endregion

    #region NEW W3C TraceParent/TraceState Tests

    [TestMethod]
    public void GetContext_WithValidW3CTraceParent_ReturnsParsedContext()
    {
        // Arrange
        const string traceParent = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
        _testObject.TraceParent = traceParent;

        // Act
        var context = ((IWithTracing)_testObject).GetContext();

        // Assert
        Assert.AreEqual("4bf92f3577b34da6a3ce929d0e0e4736", context.TraceId.ToHexString());
        Assert.AreEqual("00f067aa0ba902b7", context.SpanId.ToHexString());
        Assert.AreEqual(ActivityTraceFlags.Recorded, context.TraceFlags);
    }

    [TestMethod]
    public void GetContext_WithValidW3CTraceParentAndTraceState_ReturnsParsedContext()
    {
        // Arrange
        const string traceParent = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
        const string traceState = "convoysamplingpriority=1,something=42";
        _testObject.TraceParent = traceParent;
        _testObject.TraceState = traceState;

        // Act
        var context = ((IWithTracing)_testObject).GetContext();

        // Assert
        Assert.AreEqual("4bf92f3577b34da6a3ce929d0e0e4736", context.TraceId.ToHexString());
        Assert.AreEqual("00f067aa0ba902b7", context.SpanId.ToHexString());
        Assert.IsTrue(context.TraceState?.Contains("convoysamplingpriority=1"));
    }

    [TestMethod]
    public void GetContext_WithInvalidTraceParent_FallbacksToRandom()
    {
        // Arrange
        _testObject.TraceParent = "invalid-format";

        // Act
        var context = ((IWithTracing)_testObject).GetContext();

        // Assert
        Assert.AreEqual(32, context.TraceId.ToHexString().Length);
        Assert.AreEqual(16, context.SpanId.ToHexString().Length);
    }

    [TestMethod]
    public void AddTracing_WithActivity_SetsW3CProperties()
    {
        // Arrange
        using var activity = _activitySource.StartActivity(GetType().Name);
        activity?.SetBaggage("key", "value");

        // Act
        ((IWithTracing)_testObject).AddTracing(activity);

        // Assert
        Assert.IsNotNull(_testObject.TraceParent);
        Assert.AreEqual(activity?.TraceStateString, _testObject.TraceState);
    }

    [TestMethod]
    public void AddTracing_WithTraceParentString_SetsProperty()
    {
        // Arrange
        const string traceParent = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";

        // Act
        ((IWithTracing)_testObject).AddTracing(traceParent);

        // Assert
        Assert.AreEqual(traceParent, _testObject.TraceParent);
    }

    [TestMethod]
    public void AddTracing_WithTraceParentAndTraceState_SetsBoth()
    {
        // Arrange
        const string traceParent = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
        const string traceState = "custom=123";

        // Act
        ((IWithTracing)_testObject).AddTracing(traceParent: traceParent, traceState: traceState);

        // Assert
        Assert.AreEqual(traceParent, _testObject.TraceParent);
        Assert.AreEqual(traceState, _testObject.TraceState);
    }

    [TestMethod]
    public void AddTracing_NullCheck_DoesNotOverwriteExisting()
    {
        // Arrange
        _testObject.TraceParent = "00-existing-traceparent-xxx";
        _testObject.TraceState = "existing-state";

        // Act
        ((IWithTracing)_testObject).AddTracing(traceParent: null, traceState: null);

        // Assert
        Assert.AreEqual("00-existing-traceparent-xxx", _testObject.TraceParent);
        Assert.AreEqual("existing-state", _testObject.TraceState);
    }

    [TestMethod]
    public void StartActivity_WithW3CContext_UsesCorrectParentContext()
    {
        // Arrange
        _testObject.TraceParent = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";

        // Act
        using var activity = ((IWithTracing)_testObject).StartActivity("ChildActivity", _activitySource);

        // Assert
        Assert.IsNotNull(activity);
        Assert.AreEqual("00f067aa0ba902b7", activity.ParentSpanId.ToHexString());
    }

    [TestMethod]
    public void StartActivity_WithNoContext_CreatesNewTrace()
    {
        // Arrange
        _testObject.TraceParent = null;
        _testObject.TraceState = null;

        // Act
        using var activity = ((IWithTracing)_testObject).StartActivity("NewTraceActivity", _activitySource);

        // Assert
        Assert.IsNotNull(activity);
    }

    #endregion

    #region TraceParent & TraceState Property Tests

    [TestMethod]
    public void TraceParent_Setter_StoresValue()
    {
        // Arrange
        const string expected = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";

        // Act
        _testObject.TraceParent = expected;

        // Assert
        Assert.AreEqual(expected, _testObject.TraceParent);
    }

    [TestMethod]
    public void TraceState_Setter_StoresValue()
    {
        // Arrange
        const string expected = "convoysamplingpriority=1,custom=42";

        // Act
        _testObject.TraceState = expected;

        // Assert
        Assert.AreEqual(expected, _testObject.TraceState);
    }

    [TestMethod]
    public void TraceParent_Getter_DefaultImplementation_ReturnsNull()
    {
        // Arrange

        // Act & Assert
        Assert.IsNull(((IWithTracing)_testObject).TraceParent);
        Assert.IsNull(((IWithTracing)_testObject).TraceState);
    }

    [TestMethod]
    public void TraceParent_OverwrittenImplementation_ReturnsSetValue()
    {
        // Arrange
        var testObject = new TestTracingClass();
        const string expected = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
        testObject.TraceParent = expected;

        // Act & Assert
        Assert.AreEqual(expected, testObject.TraceParent);
    }

    [TestMethod]
    public void TraceState_NullValue_IsStored()
    {
        // Arrange
        _testObject.TraceState = null;

        // Assert
        Assert.IsNull(_testObject.TraceState);
    }

    [TestMethod]
    public void TraceState_EmptyString_IsStored()
    {
        // Arrange
        _testObject.TraceState = "";

        // Assert
        Assert.AreEqual("", _testObject.TraceState);
    }

    [TestMethod]
    public void TraceParent_SetMultipleTimes_LastValueWins()
    {
        // Arrange
        const string value1 = "00-1234567890abcdef1234567890abcdef-1234567890abcdef-01";
        const string value2 = "00-abcdef1234567890abcdef12345678-fedcba0987654321-01";

        // Act
        _testObject.TraceParent = value1;
        _testObject.TraceParent = value2;

        // Assert
        Assert.AreEqual(value2, _testObject.TraceParent);
    }

    [TestMethod]
    public void TraceParent_InvalidW3CFormat_IsStored()
    {
        // Arrange
        const string invalid = "00-invalid-too-short";

        // Act
        _testObject.TraceParent = invalid;

        // Assert
        Assert.AreEqual(invalid, _testObject.TraceParent);
    }

    [TestMethod]
    public void TraceState_InvalidFormat_IsStored()
    {
        // Arrange
        const string invalid = "invalid§format";

        // Act
        _testObject.TraceState = invalid;

        // Assert
        Assert.AreEqual(invalid, _testObject.TraceState);
    }

    [TestMethod]
    public void Properties_WorkTogether()
    {
        // Arrange
        const string traceParent = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";
        const string traceState = "convoysamplingpriority=1";

        // Act
        _testObject.TraceParent = traceParent;
        _testObject.TraceState = traceState;

        // Assert
        Assert.AreEqual(traceParent, _testObject.TraceParent);
        Assert.AreEqual(traceState, _testObject.TraceState);
    }

    #endregion

    // Test implementation of IWithTracing
    private class TestTracingClass : IWithTracing
    {
        public string? TraceId { get; set; }
        public string? SpanId { get; set; }

        public string? TraceParent { get; set; }
        public string? TraceState { get; set; }
    }
}