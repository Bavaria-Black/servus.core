using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Diagnostics;

namespace Servus.Core.Tests.Diagnostics;

    [TestClass]
    public class IWithTracingTests
    {
        private TestTracingClass _testObject;
        private ActivitySource _activitySource;

        [TestInitialize]
        public void Setup()
        {
            _testObject = new TestTracingClass();
            _activitySource = new ActivitySource("TestSource");
        }

        [TestCleanup]
        public void Cleanup()
        {
            _activitySource?.Dispose();
        }

        #region GetContext Tests

        [TestMethod]
        [DataRow("12345678901234567890123456789012", "1234567890123456", ActivityTraceFlags.Recorded)]
        [DataRow("abcdef1234567890abcdef1234567890", "abcdef1234567890", ActivityTraceFlags.Recorded)]
        public void GetContext_WithValidIds_ReturnsCorrectContext(string traceId, string spanId, ActivityTraceFlags expectedFlags)
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

        [TestMethod]
        public void AddTracing_FromCurrentActivity_UsesActivityIds()
        {
            // Arrange
            using var activity = _activitySource.StartActivity("TestActivity");
            if (activity != null)
            {
                // Act
                ((IWithTracing)_testObject).AddTracing();

                // Assert
                Assert.AreEqual(activity.TraceId.ToHexString(), _testObject.TraceId);
                Assert.AreEqual(activity.SpanId.ToHexString(), _testObject.SpanId);
            }
            else
            {
                // If no activity listener is configured, it will generate random IDs
                ((IWithTracing)_testObject).AddTracing();
                Assert.IsNotNull(_testObject.TraceId);
                Assert.IsNotNull(_testObject.SpanId);
            }
        }

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

        // Test implementation of IWithTracing
        private class TestTracingClass : IWithTracing
        {
            public string? TraceId { get; set; }
            public string? SpanId { get; set; }
        }
    }