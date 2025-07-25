using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Collections;

namespace Servus.Core.Tests.Collections;


    [TestClass]
    public class TypeRegistryTests
    {
        #region Add<TKey> Tests

        [TestMethod]
        public void Add_Generic_WithValidTypeAndValue_AddsSuccessfully()
        {
            // Arrange
            var registry = new TypeRegistry<string>();
            var testValue = "test value";

            // Act
            registry.Add<int>(testValue);

            // Assert
            var retrievedValue = registry.Get<int>();
            Assert.AreEqual(testValue, retrievedValue);
        }

        [TestMethod]
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
            Assert.AreEqual(secondValue, retrievedValue);
        }

        [TestMethod]
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
            Assert.AreEqual(stringValue, registry.Get<string>());
            Assert.AreEqual(intValue, registry.Get<int>());
        }

        [TestMethod]
        public void Add_Generic_WithNullValue_StoresNull()
        {
            // Arrange
            var registry = new TypeRegistry<string>();

            // Act
            registry.Add<int>(null!);

            // Assert
            var retrievedValue = registry.Get<int>();
            Assert.IsNull(retrievedValue);
        }

        [TestMethod]
        public void Add_Generic_WithValueTypes_WorksCorrectly()
        {
            // Arrange
            var registry = new TypeRegistry<int>();

            // Act
            registry.Add<string>(42);
            registry.Add<double>(100);

            // Assert
            Assert.AreEqual(42, registry.Get<string>());
            Assert.AreEqual(100, registry.Get<double>());
        }

        #endregion

        #region Add(Type, TValue) Tests

        [TestMethod]
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
            Assert.AreEqual(testValue, retrievedValue);
        }

        [TestMethod]
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
            Assert.AreEqual(secondValue, retrievedValue);
        }

        [TestMethod]
        public void Add_WithType_WithNullType_ThrowsArgumentNullException()
        {
            // Arrange
            var registry = new TypeRegistry<string>();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => registry.Add(null!, "value"));
        }

        [TestMethod]
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
            Assert.AreSame(listValue, registry.Get(listType));
            Assert.AreSame(dictValue, registry.Get(dictType));
        }

        #endregion

        #region Get<TKey> Tests

        [TestMethod]
        public void Get_Generic_WithExistingType_ReturnsValue()
        {
            // Arrange
            var registry = new TypeRegistry<string>();
            var testValue = "test value";
            registry.Add<int>(testValue);

            // Act
            var result = registry.Get<int>();

            // Assert
            Assert.AreEqual(testValue, result);
        }

        [TestMethod]
        public void Get_Generic_WithNonExistingType_ThrowsKeyNotFoundException()
        {
            // Arrange
            var registry = new TypeRegistry<string>();

            // Act & Assert
            Assert.ThrowsExactly<KeyNotFoundException>(() => registry.Get<int>());
        }

        [TestMethod]
        public void Get_Generic_WithDifferentGenericParameters_TreatedAsDifferentTypes()
        {
            // Arrange
            var registry = new TypeRegistry<string>();
            registry.Add<List<int>>("int list");
            registry.Add<List<string>>("string list");

            // Act & Assert
            Assert.AreEqual("int list", registry.Get<List<int>>());
            Assert.AreEqual("string list", registry.Get<List<string>>());
        }

        #endregion

        #region Get(Type) Tests

        [TestMethod]
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
            Assert.AreEqual(testValue, result);
        }

        [TestMethod]
        public void Get_WithType_WithNonExistingType_ThrowsKeyNotFoundException()
        {
            // Arrange
            var registry = new TypeRegistry<string>();
            var keyType = typeof(int);

            // Act & Assert
            Assert.ThrowsExactly<KeyNotFoundException>(() => registry.Get(keyType));
        }

        [TestMethod]
        public void Get_WithType_WithNullType_ThrowsArgumentNullException()
        {
            // Arrange
            var registry = new TypeRegistry<string>();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => registry.Get(null!));
        }

        #endregion

        #region GetOrAdd<TKey> Tests

        [TestMethod]
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
            Assert.AreEqual(existingValue, result);
        }

        [TestMethod]
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
            Assert.AreEqual(factoryValue, result);
            Assert.IsTrue(factoryCalled);
            Assert.AreEqual(factoryValue, registry.Get<int>()); // Verify it was stored
        }

        [TestMethod]
        public void GetOrAdd_Generic_WithNullFactory_ThrowsArgumentNullException()
        {
            // Arrange
            var registry = new TypeRegistry<string>();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => registry.GetOrAdd<int>(null!));
        }

        [TestMethod]
        public void GetOrAdd_Generic_FactoryReturnsNull_StoresAndReturnsNull()
        {
            // Arrange
            var registry = new TypeRegistry<string>();

            // Act
            var result = registry.GetOrAdd<int>(() => null!);

            // Assert
            Assert.IsNull(result);
            Assert.IsNull(registry.Get<int>());
        }

        #endregion

        #region GetOrAdd(Type, Func<TValue>) Tests

        [TestMethod]
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
            Assert.AreEqual(existingValue, result);
        }

        [TestMethod]
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
            Assert.AreEqual(factoryValue, result);
            Assert.IsTrue(factoryCalled);
            Assert.AreEqual(factoryValue, registry.Get(keyType)); // Verify it was stored
        }

        [TestMethod]
        public void GetOrAdd_WithType_WithNullType_ThrowsArgumentNullException()
        {
            // Arrange
            var registry = new TypeRegistry<string>();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => 
                registry.GetOrAdd(null!, () => "value"));
        }

        [TestMethod]
        public void GetOrAdd_WithType_WithNullFactory_ThrowsArgumentNullException()
        {
            // Arrange
            var registry = new TypeRegistry<string>();
            var keyType = typeof(int);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => 
                registry.GetOrAdd(keyType, null!));
        }

        #endregion

        #region Thread Safety Tests

        [TestMethod]
        public void TypeRegistry_ConcurrentAccess_IsThreadSafe()
        {
            // Arrange
            var registry = new TypeRegistry<int>();
            var tasks = new Task[10];
            var results = new int[10];

            // Act
            for (int i = 0; i < 10; i++)
            {
                int taskIndex = i;
                tasks[i] = Task.Run(() =>
                {
                    // Each task adds a value for a different type and then retrieves it
                    var typeKey = Type.GetType($"System.Int{taskIndex + 16}") ?? typeof(int); // Use different types
                    registry.Add(typeKey, taskIndex);
                    results[taskIndex] = registry.Get(typeKey);
                });
            }

            Task.WaitAll(tasks);

            // Assert
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(i, results[i]);
            }
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void TypeRegistry_CompleteWorkflow_WorksCorrectly()
        {
            // Arrange
            var registry = new TypeRegistry<object>();

            // Act & Assert - Add different types
            registry.Add<string>("string value");
            registry.Add<int>(42);
            registry.Add<List<string>>(new List<string> { "item1", "item2" });

            // Verify all values can be retrieved
            Assert.AreEqual("string value", registry.Get<string>());
            Assert.AreEqual(42, registry.Get<int>());
            Assert.IsInstanceOfType(registry.Get<List<string>>(), typeof(List<string>));

            // Test GetOrAdd with existing
            var existingString = registry.GetOrAdd<string>(() => "should not be called");
            Assert.AreEqual("string value", existingString);

            // Test GetOrAdd with new type
            var newValue = registry.GetOrAdd<double>(() => 3.14);
            Assert.AreEqual(3.14, newValue);
            Assert.AreEqual(3.14, registry.Get<double>());

            // Test overwriting existing value
            registry.Add<string>("new string value");
            Assert.AreEqual("new string value", registry.Get<string>());
        }

        [TestMethod]
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

            Assert.AreSame(person1, retrievedEmployee);
            Assert.AreSame(person2, retrievedCustomer);
            Assert.AreEqual("John", retrievedEmployee.Name);
            Assert.AreEqual("Jane", retrievedCustomer.Name);
        }

        [TestMethod]
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
            Assert.AreEqual("int list", registry.Get<List<int>>());
            Assert.AreEqual("string list", registry.Get<List<string>>());
            Assert.AreEqual("string-int dict", registry.Get<Dictionary<string, int>>());
            Assert.AreEqual("int-string dict", registry.Get<Dictionary<int, string>>());
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

        #endregion
    }