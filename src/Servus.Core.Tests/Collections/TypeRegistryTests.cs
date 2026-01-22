using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Servus.Core.Collections;

namespace Servus.Core.Tests.Collections;

[CollectionDefinition("SerialTests", DisableParallelization = true)]
public class SerialTestCollection;

[Collection("SerialTests")]
public class TypeRegistryTests
{
    #region Add<TKey> Tests

    [Fact]
    public void Add_Generic_WithValidTypeAndValue_AddsSuccessfully()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var testValue = "test value";

        // Act
        registry.Add<int>(testValue);

        // Assert
        var retrievedValue = registry.Get<int>();
        Assert.Equal(testValue, retrievedValue);
    }

    [Fact]
    public void Add_Generic_WithSameTypeTwice_UpdatesValue()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var firstValue = "first value";
        var secondValue = "second value";

        // Act
        registry.Add<int>(firstValue);
        registry.Add<int>(secondValue);

        // Assert
        var retrievedValue = registry.Get<int>();
        Assert.Equal(secondValue, retrievedValue);
    }

    [Fact]
    public void Add_Generic_WithDifferentTypes_StoresSeparately()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var stringValue = "string value";
        var intValue = "int value";

        // Act
        registry.Add<string>(stringValue);
        registry.Add<int>(intValue);

        // Assert
        Assert.Equal(stringValue, registry.Get<string>());
        Assert.Equal(intValue, registry.Get<int>());
    }

    [Fact]
    public void Add_Generic_WithNullValue_StoresNull()
    {
        // Arrange
        var registry = new TypeRegistry<string>();

        // Act
        registry.Add<int>(null!);

        // Assert
        var retrievedValue = registry.Get<int>();
        Assert.Null(retrievedValue);
    }

    [Fact]
    public void Add_Generic_WithValueTypes_WorksCorrectly()
    {
        // Arrange
        var registry = new TypeRegistry<int>();

        // Act
        registry.Add<string>(42);
        registry.Add<double>(100);

        // Assert
        Assert.Equal(42, registry.Get<string>());
        Assert.Equal(100, registry.Get<double>());
    }

    #endregion

    #region Add(Type, TValue) Tests

    [Fact]
    public void Add_WithType_WithValidTypeAndValue_AddsSuccessfully()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var testValue = "test value";
        var keyType = typeof(int);

        // Act
        registry.Add(keyType, testValue);

        // Assert
        var retrievedValue = registry.Get(keyType);
        Assert.Equal(testValue, retrievedValue);
    }

    [Fact]
    public void Add_WithType_WithSameTypeTwice_UpdatesValue()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var firstValue = "first value";
        var secondValue = "second value";
        var keyType = typeof(int);

        // Act
        registry.Add(keyType, firstValue);
        registry.Add(keyType, secondValue);

        // Assert
        var retrievedValue = registry.Get(keyType);
        Assert.Equal(secondValue, retrievedValue);
    }

    [Fact]
    public void Add_WithType_WithNullType_ThrowsArgumentNullException()
    {
        // Arrange
        var registry = new TypeRegistry<string>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => registry.Add(null!, "value"));
    }

    [Fact]
    public void Add_WithType_WithComplexTypes_WorksCorrectly()
    {
        // Arrange
        var registry = new TypeRegistry<object>();
        var listType = typeof(List<int>);
        var dictType = typeof(Dictionary<string, int>);
        var listValue = new List<int> { 1, 2, 3 };
        var dictValue = new Dictionary<string, int> { { "key", 42 } };

        // Act
        registry.Add(listType, listValue);
        registry.Add(dictType, dictValue);

        // Assert
        Assert.Same(listValue, registry.Get(listType));
        Assert.Same(dictValue, registry.Get(dictType));
    }

    #endregion

    #region Get<TKey> Tests

    [Fact]
    public void Get_Generic_WithExistingType_ReturnsValue()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var testValue = "test value";
        registry.Add<int>(testValue);

        // Act
        var result = registry.Get<int>();

        // Assert
        Assert.Equal(testValue, result);
    }

    [Fact]
    public void Get_Generic_WithNonExistingType_ThrowsKeyNotFoundException()
    {
        // Arrange
        var registry = new TypeRegistry<string>();

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => registry.Get<int>());
    }

    [Fact]
    public void Get_Generic_WithDifferentGenericParameters_TreatedAsDifferentTypes()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        registry.Add<List<int>>("int list");
        registry.Add<List<string>>("string list");

        // Act & Assert
        Assert.Equal("int list", registry.Get<List<int>>());
        Assert.Equal("string list", registry.Get<List<string>>());
    }

    #endregion

    #region Get(Type) Tests

    [Fact]
    public void Get_WithType_WithExistingType_ReturnsValue()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var testValue = "test value";
        var keyType = typeof(int);
        registry.Add(keyType, testValue);

        // Act
        var result = registry.Get(keyType);

        // Assert
        Assert.Equal(testValue, result);
    }

    [Fact]
    public void Get_WithType_WithNonExistingType_ThrowsKeyNotFoundException()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var keyType = typeof(int);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => registry.Get(keyType));
    }

    [Fact]
    public void Get_WithType_WithNullType_ThrowsArgumentNullException()
    {
        // Arrange
        var registry = new TypeRegistry<string>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => registry.Get(null!));
    }

    #endregion

    #region GetOrAdd<TKey> Tests

    [Fact]
    public void GetOrAdd_Generic_WithExistingType_ReturnsExistingValue()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var existingValue = "existing value";
        var factoryValue = "factory value";
        registry.Add<int>(existingValue);

        // Act
        var result = registry.GetOrAdd<int>(() => factoryValue);

        // Assert
        Assert.Equal(existingValue, result);
    }

    [Fact]
    public void GetOrAdd_Generic_WithNonExistingType_CallsFactoryAndReturnsValue()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var factoryValue = "factory value";
        var factoryCalled = false;

        // Act
        var result = registry.GetOrAdd<int>(() =>
        {
            factoryCalled = true;
            return factoryValue;
        });

        // Assert
        Assert.Equal(factoryValue, result);
        Assert.True(factoryCalled);
        Assert.Equal(factoryValue, registry.Get<int>()); // Verify it was stored
    }

    [Fact]
    public void GetOrAdd_Generic_WithNullFactory_ThrowsArgumentNullException()
    {
        // Arrange
        var registry = new TypeRegistry<string>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => registry.GetOrAdd<int>(null!));
    }

    [Fact]
    public void GetOrAdd_Generic_FactoryReturnsNull_StoresAndReturnsNull()
    {
        // Arrange
        var registry = new TypeRegistry<string>();

        // Act
        var result = registry.GetOrAdd<int>(() => null!);

        // Assert
        Assert.Null(result);
        Assert.Null(registry.Get<int>());
    }

    #endregion

    #region GetOrAdd(Type, Func<TValue>) Tests

    [Fact]
    public void GetOrAdd_WithType_WithExistingType_ReturnsExistingValue()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var existingValue = "existing value";
        var factoryValue = "factory value";
        var keyType = typeof(int);
        registry.Add(keyType, existingValue);

        // Act
        var result = registry.GetOrAdd(keyType, () => factoryValue);

        // Assert
        Assert.Equal(existingValue, result);
    }

    [Fact]
    public void GetOrAdd_WithType_WithNonExistingType_CallsFactoryAndReturnsValue()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var factoryValue = "factory value";
        var keyType = typeof(int);
        var factoryCalled = false;

        // Act
        var result = registry.GetOrAdd(keyType, () =>
        {
            factoryCalled = true;
            return factoryValue;
        });

        // Assert
        Assert.Equal(factoryValue, result);
        Assert.True(factoryCalled);
        Assert.Equal(factoryValue, registry.Get(keyType)); // Verify it was stored
    }

    [Fact]
    public void GetOrAdd_WithType_WithNullType_ThrowsArgumentNullException()
    {
        // Arrange
        var registry = new TypeRegistry<string>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            registry.GetOrAdd(null!, () => "value"));
    }

    [Fact]
    public void GetOrAdd_WithType_WithNullFactory_ThrowsArgumentNullException()
    {
        // Arrange
        var registry = new TypeRegistry<string>();
        var keyType = typeof(int);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            registry.GetOrAdd(keyType, null!));
    }

    #endregion

    #region Thread Safety Tests

    [Fact]
    public async Task TypeRegistry_ConcurrentSameType_LastWriteWins()
    {
        // Arrange
        var registry = new TypeRegistry<int>();
        var taskCount = 100;
        var sameType = typeof(DummyType<string>);

        // Act - All tasks write to the SAME type key
        var tasks = Enumerable.Range(0, taskCount).Select(i => Task.Run(() => { registry.Add(sameType, i); }))
            .ToArray();

        await Task.WhenAll(tasks);

        // Assert - Should have exactly one value (last write wins)
        var result = registry.Get(sameType);
        Assert.InRange(result, 0, taskCount - 1);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task TypeRegistry_ConcurrentAccess_IsThreadSafe(int concurrentTasks)
    {
        // Arrange
        var registry = new TypeRegistry<int>();
        var results = new ConcurrentDictionary<int, int>();

        // Create unique types dynamically
        var uniqueTypes = Enumerable.Range(0, concurrentTasks)
            .Select(CreateUniqueTypeMarker)
            .ToArray();

        // Act
        var tasks = Enumerable.Range(0, concurrentTasks).Select(i => Task.Run(() =>
        {
            registry.Add(uniqueTypes[i], i);
            results.TryAdd(i, registry.Get(uniqueTypes[i]));
        })).ToArray();

        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(concurrentTasks, results.Count);
        for (var i = 0; i < concurrentTasks; i++)
        {
            Assert.Equal(i, results[i]);
        }
    }

    private static Type CreateUniqueTypeMarker(int index)
    {
        var currentType = typeof(int);

        for (var i = 0; i < index; i++)
        {
            currentType = typeof(DummyType<>).MakeGenericType(currentType);
        }

        return currentType;
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void TypeRegistry_CompleteWorkflow_WorksCorrectly()
    {
        // Arrange
        var registry = new TypeRegistry<object>();

        // Act & Assert - Add different types
        registry.Add<string>("string value");
        registry.Add<int>(42);
        registry.Add<List<string>>(new List<string> { "item1", "item2" });

        // Verify all values can be retrieved
        Assert.Equal("string value", registry.Get<string>());
        Assert.Equal(42, registry.Get<int>());
        Assert.IsType<List<string>>(registry.Get<List<string>>());

        // Test GetOrAdd with existing
        var existingString = registry.GetOrAdd<string>(() => "should not be called");
        Assert.Equal("string value", existingString);

        // Test GetOrAdd with new type
        var newValue = registry.GetOrAdd<double>(() => 3.14);
        Assert.Equal(3.14, newValue);
        Assert.Equal(3.14, registry.Get<double>());

        // Test overwriting existing value
        registry.Add<string>("new string value");
        Assert.Equal("new string value", registry.Get<string>());
    }

    [Fact]
    public void TypeRegistry_WithCustomObjects_WorksCorrectly()
    {
        // Arrange
        var registry = new TypeRegistry<Person>();
        var person1 = new Person { Name = "John", Age = 30 };
        var person2 = new Person { Name = "Jane", Age = 25 };

        // Act
        registry.Add<Employee>(person1);
        registry.Add<Customer>(person2);

        // Assert
        var retrievedEmployee = registry.Get<Employee>();
        var retrievedCustomer = registry.Get<Customer>();

        Assert.Same(person1, retrievedEmployee);
        Assert.Same(person2, retrievedCustomer);
        Assert.Equal("John", retrievedEmployee.Name);
        Assert.Equal("Jane", retrievedCustomer.Name);
    }

    [Fact]
    public void TypeRegistry_WithGenericTypes_DistinguishesCorrectly()
    {
        // Arrange
        var registry = new TypeRegistry<string>();

        // Act
        registry.Add<List<int>>("int list");
        registry.Add<List<string>>("string list");
        registry.Add<Dictionary<string, int>>("string-int dict");
        registry.Add<Dictionary<int, string>>("int-string dict");

        // Assert
        Assert.Equal("int list", registry.Get<List<int>>());
        Assert.Equal("string list", registry.Get<List<string>>());
        Assert.Equal("string-int dict", registry.Get<Dictionary<string, int>>());
        Assert.Equal("int-string dict", registry.Get<Dictionary<int, string>>());
    }

    #endregion

    #region Helper Classes for Testing

    private class Person
    {
        public string Name { get; init; } = string.Empty;
        public int Age { get; set; }
    }

    private class Employee : Person
    {
        public int EmployeeId { get; init; } = 0;
    }

    private class Customer : Person
    {
        public int CustomerId { get; init; } = 0;
    }

    public sealed class DummyType<T>
    {
        public static implicit operator Type(DummyType<T> _) => typeof(T);

        public override string ToString() => $"DummyType<{typeof(T).Name}>";

        public override bool Equals(object? obj) => obj is DummyType<T> or Type _ and T;

        public override int GetHashCode() => typeof(T).GetHashCode();
    }

    #endregion
}