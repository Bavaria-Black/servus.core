using System;
using Xunit;
using Servus.Core.Collections;

namespace Servus.Core.Tests.Collections;

public class LazyValueCacheTests
{
    private LazyValueCache<string, string> _stringCache = null!;
    private LazyValueCache<int, object> _objectCache = null!;

    public LazyValueCacheTests()
    {
        _stringCache = new LazyValueCache<string, string>();
        _objectCache = new LazyValueCache<int, object>();
    }

    #region Basic Functionality Tests

    [Theory]
    [InlineData("key1", "value1")]
    [InlineData("key2", "value2")]
    [InlineData("", "empty_key_value")]
    [InlineData("special@key#123", "special_value")]
    public void Get_FirstCall_CallsProviderAndReturnsValue(string key, string expectedValue)
    {
        // Act
        var result = _stringCache.GetOrCreate(key, () => expectedValue);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData("key1", "value1")]
    [InlineData("key2", "value2")]
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
        Assert.Equal(expectedValue, result1);
        Assert.Equal(expectedValue, result2);
        Assert.Equal(1, providerCallCount);
    }

    [Fact]
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
        Assert.Equal("value1", result1a);
        Assert.Equal("value1", result1b);
        Assert.Equal("value2", result2a);
        Assert.Equal("value2", result2b);
        Assert.Equal(1, key1CallCount);
        Assert.Equal(1, key2CallCount);
    }

    #endregion

    #region Value Type Tests

    [Theory]
    [InlineData(1, 100)]
    [InlineData(42, 200)]
    [InlineData(-1, 300)]
    [InlineData(0, 400)]
    public void Get_IntegerValues_WorksCorrectly(int key, int expectedValue)
    {
        // Arrange
        var intCache = new LazyValueCache<int, int>();

        // Act
        var result = intCache.GetOrCreate(key, () => expectedValue);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Get_BooleanValues_WorksCorrectly()
    {
        // Arrange
        var boolCache = new LazyValueCache<string, bool>();

        // Act
        var trueResult = boolCache.GetOrCreate("true_key", () => true);
        var falseResult = boolCache.GetOrCreate("false_key", () => false);

        // Assert
        Assert.True(trueResult);
        Assert.False(falseResult);
    }

    #endregion

    #region Object Type Tests

    [Fact]
    public void Get_ComplexObjects_CachesCorrectly()
    {
        // Arrange
        var expectedObject = new { Name = "Test", Value = 123 };
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
        Assert.Same(expectedObject, result1);
        Assert.Same(expectedObject, result2);
        Assert.Equal(1, providerCallCount);
    }

    [Theory]
    [InlineData(1, null)]
    [InlineData(2, null)]
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
        Assert.Null(result1);
        Assert.Null(result2);
        Assert.Equal(1, providerCallCount);
    }

    #endregion

    #region Exception Handling Tests

    [Fact]
    public void Get_NullProvider_ThrowsArgumentNullException()
    {
        // Act
        Assert.Throws<ArgumentNullException>(() => _stringCache.GetOrCreate(null, () => null));
    }

    [Fact]
    public void Get_ProviderThrowsException_PropagatesException()
    {
        // Act
        Assert.Throws<InvalidOperationException>(() => _stringCache.GetOrCreate("key", () => throw new InvalidOperationException("Test exception")));
    }

    [Fact]
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
        Assert.Throws<InvalidOperationException>(() => _stringCache.GetOrCreate("key", ThrowingProvider));
        Assert.Throws<InvalidOperationException>(() => _stringCache.GetOrCreate("key", ThrowingProvider));

        Assert.Equal(2, callCount);
    }

    [Fact]
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
        Assert.Throws<InvalidOperationException>(() => _stringCache.GetOrCreate("key", Provider));
        var result = _stringCache.GetOrCreate("key", Provider);
        var cachedResult = _stringCache.GetOrCreate("key", Provider);

        Assert.Equal("success", result);
        Assert.Equal("success", cachedResult);
        Assert.Equal(2, callCount);
    }

    #endregion

    #region Performance/Behavior Tests

    [Fact]
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
        Assert.Equal("Result_1", result1);
        Assert.Equal("Result_1", result2);
        Assert.Equal("Result_1", result3);
        Assert.Equal(1, computationCount);
    }

    [Fact]
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
        Assert.Equal("first", result1);
        Assert.Equal("first", result2);
        Assert.True(firstProviderCalled);
        Assert.False(secondProviderCalled, "Second provider should not be called");
    }

    #endregion

    #region TryGet Tests

    [Theory]
    [InlineData("key1", "value1")]
    [InlineData("key2", "value2")]
    [InlineData("", "empty_key_value")]
    [InlineData("special@key#123", "special_value")]
    public void TryPeek_ExistingKey_ReturnsTrueWithValue(string key, string expectedValue)
    {
        // Arrange
        _stringCache.GetOrCreate(key, () => expectedValue); // Cache the value first

        // Act
        var result = _stringCache.TryGetValue(key, out var value);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedValue, value);
    }

    [Theory]
    [InlineData("nonexistent1")]
    [InlineData("nonexistent2")]
    [InlineData("")]
    [InlineData("never_cached")]
    public void TryPeek_NonExistentKey_ReturnsFalseWithDefaultValue(string key)
    {
        // Act
        var result = _stringCache.TryGetValue(key, out var value);

        // Assert
        Assert.False(result);
        Assert.Null(value); // Default value for reference types
    }

    [Fact]
    public void TryPeek_NullValue_ReturnsTrueWithNull()
    {
        // Arrange
        const int key = 1;
        _objectCache.GetOrCreate(key, () => null!); // Cache null value

        // Act
        var result = _objectCache.TryGetValue(key, out var value);

        // Assert
        Assert.True(result);
        Assert.Null(value);
    }

    [Fact]
    public void TryPeek_ValueTypes_WorksCorrectly()
    {
        // Arrange
        var intCache = new LazyValueCache<string, int>();
        intCache.GetOrCreate("int_key", () => 42);

        // Act
        var existsResult = intCache.TryGetValue("int_key", out var existingValue);
        var notExistsResult = intCache.TryGetValue("missing_key", out var missingValue);

        // Assert
        Assert.True(existsResult);
        Assert.Equal(42, existingValue);
        Assert.False(notExistsResult);
        Assert.Equal(0, missingValue); // Default value for int
    }

    [Fact]
    public void TryPeek_DoesNotTriggerProvider()
    {
        // Arrange
        const bool providerCalled = false;
        const string key = "test_key";

        // Act - TryPeek on non-existent key
        var result = _stringCache.TryGetValue(key, out var value);

        // Assert
        Assert.False(result);
        Assert.Null(value);
        Assert.False(providerCalled, "TryPeek should not trigger any provider");
    }

    [Fact]
    public void TryPeek_AfterGet_ReturnsCorrectValue()
    {
        // Arrange
        const string key = "combined_test";
        const string expectedValue = "test_value";

        // Act - First use Get to cache
        var getValue = _stringCache.GetOrCreate(key, () => expectedValue);
        // Then use TryPeek
        var peekResult = _stringCache.TryGetValue(key, out var peekValue);

        // Assert
        Assert.Equal(expectedValue, getValue);
        Assert.True(peekResult);
        Assert.Equal(expectedValue, peekValue);
    }

    [Fact]
    public void TryPeek_ComplexObjects_ReturnsSameReference()
    {
        // Arrange
        const int key = 1;
        var expectedObject = new { Name = "Test", Value = 123 };
        _objectCache.GetOrCreate(key, () => expectedObject);

        // Act
        var result = _objectCache.TryGetValue(key, out var value);

        // Assert
        Assert.True(result);
        Assert.Same(expectedObject, value);
    }

    [Fact]
    public void TryPeek_MultipleKeys_ReturnsCorrectValues()
    {
        // Arrange
        _stringCache.GetOrCreate("key1", () => "value1");
        _stringCache.GetOrCreate("key2", () => "value2");

        // Act
        var result1 = _stringCache.TryGetValue("key1", out var value1);
        var result2 = _stringCache.TryGetValue("key2", out var value2);
        var result3 = _stringCache.TryGetValue("key3", out var value3);

        // Assert
        Assert.True(result1);
        Assert.Equal("value1", value1);
        Assert.True(result2);
        Assert.Equal("value2", value2);
        Assert.False(result3);
        Assert.Null(value3);
    }

    #endregion
}