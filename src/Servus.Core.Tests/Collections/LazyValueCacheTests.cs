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
        var result = _stringCache.Get(key, () => expectedValue);

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
        var result1 = _stringCache.Get(key, Provider);
        // Act - Second call
        var result2 = _stringCache.Get(key, Provider);

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
        var result1a = _stringCache.Get("key1", () =>
        {
            key1CallCount++;
            return "value1";
        });
        var result2a = _stringCache.Get("key2", () =>
        {
            key2CallCount++;
            return "value2";
        });
        var result1b = _stringCache.Get("key1", () =>
        {
            key1CallCount++;
            return "value1";
        });
        var result2b = _stringCache.Get("key2", () =>
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
        var result = intCache.Get(key, () => expectedValue);

        // Assert
        Assert.AreEqual(expectedValue, result);
    }

    [TestMethod]
    public void Get_BooleanValues_WorksCorrectly()
    {
        // Arrange
        var boolCache = new LazyValueCache<string, bool>();

        // Act
        var trueResult = boolCache.Get("true_key", () => true);
        var falseResult = boolCache.Get("false_key", () => false);

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
        var result1 = _objectCache.Get(1, () =>
        {
            providerCallCount++;
            return expectedObject;
        });
        var result2 = _objectCache.Get(1, () =>
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
        var result1 = _objectCache.Get(key, () =>
        {
            providerCallCount++;
            return expectedValue;
        });
        var result2 = _objectCache.Get(key, () =>
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
        _stringCache.Get("key", null!);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Get_ProviderThrowsException_PropagatesException()
    {
        // Act
        _stringCache.Get("key", () => throw new InvalidOperationException("Test exception"));
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
        Assert.ThrowsException<InvalidOperationException>(() => _stringCache.Get("key", ThrowingProvider));
        Assert.ThrowsException<InvalidOperationException>(() => _stringCache.Get("key", ThrowingProvider));

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
        Assert.ThrowsException<InvalidOperationException>(() => _stringCache.Get("key", Provider));
        var result = _stringCache.Get("key", Provider);
        var cachedResult = _stringCache.Get("key", Provider);

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
        var result1 = _stringCache.Get("expensive", ExpensiveOperation);
        var result2 = _stringCache.Get("expensive", ExpensiveOperation);
        var result3 = _stringCache.Get("expensive", ExpensiveOperation);

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
        var result1 = _stringCache.Get("key", () =>
        {
            firstProviderCalled = true;
            return "first";
        });
        var result2 = _stringCache.Get("key", () =>
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
}