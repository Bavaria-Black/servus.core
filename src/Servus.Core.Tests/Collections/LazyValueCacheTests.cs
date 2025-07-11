using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Collections;

namespace Servus.Core.Tests.Collections;

[TestClass]
public class LazyValueCacheTests
{
    private LazyValueCache<string, string> _stringCache = null!;
    private LazyValueCache<int, object> _objectCache = null!;

    [TestInitialize]
    public void Setup()
    {
        _stringCache = new LazyValueCache<string, string>();
        _objectCache = new LazyValueCache<int, object>();
    }

    #region Basic Functionality Tests

    [TestMethod]
    [DataRow("key1", "value1")]
    [DataRow("key2", "value2")]
    [DataRow("", "empty_key_value")]
    [DataRow("special@key#123", "special_value")]
    public void Get_FirstCall_CallsProviderAndReturnsValue(string key, string expectedValue)
    {
        // Act
        var result = _stringCache.GetOrCreate(key, () => expectedValue);

        // Assert
        Assert.AreEqual(expectedValue, result);
    }

    [TestMethod]
    [DataRow("key1", "value1")]
    [DataRow("key2", "value2")]
    public void Get_SecondCall_ReturnsCachedValueWithoutCallingProvider(string key, string expectedValue)
    {
        // Arrange
        var providerCallCount = 0;

        string Provider()
        {
            providerCallCount++;
            return expectedValue;
        }

        // Act - First call
        var result1 = _stringCache.GetOrCreate(key, Provider);
        // Act - Second call
        var result2 = _stringCache.GetOrCreate(key, Provider);

        // Assert
        Assert.AreEqual(expectedValue, result1);
        Assert.AreEqual(expectedValue, result2);
        Assert.AreEqual(1, providerCallCount, "Provider should only be called once");
    }

    [TestMethod]
    public void Get_MultipleKeys_CachesIndependently()
    {
        // Arrange
        var key1CallCount = 0;
        var key2CallCount = 0;

        // Act
        var result1a = _stringCache.GetOrCreate("key1", () =>
        {
            key1CallCount++;
            return "value1";
        });
        var result2a = _stringCache.GetOrCreate("key2", () =>
        {
            key2CallCount++;
            return "value2";
        });
        var result1b = _stringCache.GetOrCreate("key1", () =>
        {
            key1CallCount++;
            return "value1";
        });
        var result2b = _stringCache.GetOrCreate("key2", () =>
        {
            key2CallCount++;
            return "value2";
        });

        // Assert
        Assert.AreEqual("value1", result1a);
        Assert.AreEqual("value1", result1b);
        Assert.AreEqual("value2", result2a);
        Assert.AreEqual("value2", result2b);
        Assert.AreEqual(1, key1CallCount, "Key1 provider should only be called once");
        Assert.AreEqual(1, key2CallCount, "Key2 provider should only be called once");
    }

    #endregion

    #region Value Type Tests

    [TestMethod]
    [DataRow(1, 100)]
    [DataRow(42, 200)]
    [DataRow(-1, 300)]
    [DataRow(0, 400)]
    public void Get_IntegerValues_WorksCorrectly(int key, int expectedValue)
    {
        // Arrange
        var intCache = new LazyValueCache<int, int>();

        // Act
        var result = intCache.GetOrCreate(key, () => expectedValue);

        // Assert
        Assert.AreEqual(expectedValue, result);
    }

    [TestMethod]
    public void Get_BooleanValues_WorksCorrectly()
    {
        // Arrange
        var boolCache = new LazyValueCache<string, bool>();

        // Act
        var trueResult = boolCache.GetOrCreate("true_key", () => true);
        var falseResult = boolCache.GetOrCreate("false_key", () => false);

        // Assert
        Assert.IsTrue(trueResult);
        Assert.IsFalse(falseResult);
    }

    #endregion

    #region Object Type Tests

    [TestMethod]
    public void Get_ComplexObjects_CachesCorrectly()
    {
        // Arrange
        var expectedObject = new {Name = "Test", Value = 123};
        var providerCallCount = 0;

        // Act
        var result1 = _objectCache.GetOrCreate(1, () =>
        {
            providerCallCount++;
            return expectedObject;
        });
        var result2 = _objectCache.GetOrCreate(1, () =>
        {
            providerCallCount++;
            return expectedObject;
        });

        // Assert
        Assert.AreSame(expectedObject, result1);
        Assert.AreSame(expectedObject, result2);
        Assert.AreEqual(1, providerCallCount);
    }

    [TestMethod]
    [DataRow(1, null)]
    [DataRow(2, null)]
    public void Get_NullValues_CachesCorrectly(int key, object expectedValue)
    {
        // Arrange
        var providerCallCount = 0;

        // Act
        var result1 = _objectCache.GetOrCreate(key, () =>
        {
            providerCallCount++;
            return expectedValue;
        });
        var result2 = _objectCache.GetOrCreate(key, () =>
        {
            providerCallCount++;
            return expectedValue;
        });

        // Assert
        Assert.IsNull(result1);
        Assert.IsNull(result2);
        Assert.AreEqual(1, providerCallCount, "Provider should only be called once even for null values");
    }

    #endregion

    #region Exception Handling Tests

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Get_NullProvider_ThrowsArgumentNullException()
    {
        // Act
        _stringCache.GetOrCreate("key", null!);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Get_ProviderThrowsException_PropagatesException()
    {
        // Act
        _stringCache.GetOrCreate("key", () => throw new InvalidOperationException("Test exception"));
    }

    [TestMethod]
    public void Get_ProviderThrowsException_DoesNotCache()
    {
        // Arrange
        var callCount = 0;

        string ThrowingProvider()
        {
            callCount++;
            throw new InvalidOperationException("Test exception");
        }

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _stringCache.GetOrCreate("key", ThrowingProvider));
        Assert.ThrowsException<InvalidOperationException>(() => _stringCache.GetOrCreate("key", ThrowingProvider));

        Assert.AreEqual(2, callCount, "Provider should be called again after exception");
    }

    [TestMethod]
    public void Get_ExceptionThenSuccess_CachesSuccessfulResult()
    {
        // Arrange
        var callCount = 0;

        string Provider()
        {
            callCount++;
            if (callCount == 1) throw new InvalidOperationException("First call fails");
            return "success";
        }

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _stringCache.GetOrCreate("key", Provider));
        var result = _stringCache.GetOrCreate("key", Provider);
        var cachedResult = _stringCache.GetOrCreate("key", Provider);

        Assert.AreEqual("success", result);
        Assert.AreEqual("success", cachedResult);
        Assert.AreEqual(2, callCount, "Provider should be called twice - once failing, once succeeding");
    }

    #endregion

    #region Performance/Behavior Tests

    [TestMethod]
    public void Get_ExpensiveOperation_OnlyComputedOnce()
    {
        // Arrange
        var computationCount = 0;

        string ExpensiveOperation()
        {
            computationCount++;
            System.Threading.Thread.Sleep(1); // Simulate work
            return $"Result_{computationCount}";
        }

        // Act
        var result1 = _stringCache.GetOrCreate("expensive", ExpensiveOperation);
        var result2 = _stringCache.GetOrCreate("expensive", ExpensiveOperation);
        var result3 = _stringCache.GetOrCreate("expensive", ExpensiveOperation);

        // Assert
        Assert.AreEqual("Result_1", result1);
        Assert.AreEqual("Result_1", result2);
        Assert.AreEqual("Result_1", result3);
        Assert.AreEqual(1, computationCount, "Expensive operation should only run once");
    }

    [TestMethod]
    public void Get_DifferentProvidersSameKey_UsesFirstResult()
    {
        // Arrange
        var firstProviderCalled = false;
        var secondProviderCalled = false;

        // Act
        var result1 = _stringCache.GetOrCreate("key", () =>
        {
            firstProviderCalled = true;
            return "first";
        });
        var result2 = _stringCache.GetOrCreate("key", () =>
        {
            secondProviderCalled = true;
            return "second";
        });

        // Assert
        Assert.AreEqual("first", result1);
        Assert.AreEqual("first", result2);
        Assert.IsTrue(firstProviderCalled);
        Assert.IsFalse(secondProviderCalled, "Second provider should not be called");
    }

    #endregion
    
    #region TryPeek Tests

        [TestMethod]
        [DataRow("key1", "value1")]
        [DataRow("key2", "value2")]
        [DataRow("", "empty_key_value")]
        [DataRow("special@key#123", "special_value")]
        public void TryPeek_ExistingKey_ReturnsTrueWithValue(string key, string expectedValue)
        {
            // Arrange
            _stringCache.GetOrCreate(key, () => expectedValue); // Cache the value first

            // Act
            var result = _stringCache.TryPeek(key, out var value);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        [DataRow("nonexistent1")]
        [DataRow("nonexistent2")]
        [DataRow("")]
        [DataRow("never_cached")]
        public void TryPeek_NonExistentKey_ReturnsFalseWithDefaultValue(string key)
        {
            // Act
            var result = _stringCache.TryPeek(key, out var value);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(value); // Default value for reference types
        }

        [TestMethod]
        public void TryPeek_NullValue_ReturnsTrueWithNull()
        {
            // Arrange
            var key = 1;
            _objectCache.GetOrCreate(key, () => null); // Cache null value

            // Act
            var result = _objectCache.TryPeek(key, out var value);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TryPeek_ValueTypes_WorksCorrectly()
        {
            // Arrange
            var intCache = new LazyValueCache<string, int>();
            intCache.GetOrCreate("int_key", () => 42);

            // Act
            var existsResult = intCache.TryPeek("int_key", out var existingValue);
            var notExistsResult = intCache.TryPeek("missing_key", out var missingValue);

            // Assert
            Assert.IsTrue(existsResult);
            Assert.AreEqual(42, existingValue);
            Assert.IsFalse(notExistsResult);
            Assert.AreEqual(0, missingValue); // Default value for int
        }

        [TestMethod]
        public void TryPeek_DoesNotTriggerProvider()
        {
            // Arrange
            var providerCalled = false;
            var key = "test_key";

            // Act - TryPeek on non-existent key
            var result = _stringCache.TryPeek(key, out var value);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(value);
            Assert.IsFalse(providerCalled, "TryPeek should not trigger any provider");
        }

        [TestMethod]
        public void TryPeek_AfterGet_ReturnsCorrectValue()
        {
            // Arrange
            var key = "combined_test";
            var expectedValue = "test_value";

            // Act - First use Get to cache
            var getValue = _stringCache.GetOrCreate(key, () => expectedValue);
            // Then use TryPeek
            var peekResult = _stringCache.TryPeek(key, out var peekValue);

            // Assert
            Assert.AreEqual(expectedValue, getValue);
            Assert.IsTrue(peekResult);
            Assert.AreEqual(expectedValue, peekValue);
        }

        [TestMethod]
        public void TryPeek_ComplexObjects_ReturnsSameReference()
        {
            // Arrange
            var key = 1;
            var expectedObject = new { Name = "Test", Value = 123 };
            _objectCache.GetOrCreate(key, () => expectedObject);

            // Act
            var result = _objectCache.TryPeek(key, out var value);

            // Assert
            Assert.IsTrue(result);
            Assert.AreSame(expectedObject, value);
        }

        [TestMethod]
        public void TryPeek_MultipleKeys_ReturnsCorrectValues()
        {
            // Arrange
            _stringCache.GetOrCreate("key1", () => "value1");
            _stringCache.GetOrCreate("key2", () => "value2");

            // Act
            var result1 = _stringCache.TryPeek("key1", out var value1);
            var result2 = _stringCache.TryPeek("key2", out var value2);
            var result3 = _stringCache.TryPeek("key3", out var value3);

            // Assert
            Assert.IsTrue(result1);
            Assert.AreEqual("value1", value1);
            Assert.IsTrue(result2);
            Assert.AreEqual("value2", value2);
            Assert.IsFalse(result3);
            Assert.IsNull(value3);
        }

        #endregion
}