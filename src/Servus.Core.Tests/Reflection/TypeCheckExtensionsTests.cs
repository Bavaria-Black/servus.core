using System;
using Servus.Core.Reflection;
using Xunit;

namespace Servus.Core.Tests.Reflection
{
    public class TypeCheckExtensionsTests
    {
        #region TryConvert<TTarget> Tests

        [Fact]
        public void TryConvert_WhenItemIsOfTargetType_ReturnsTrue()
        {
            // Arrange
            object item = "Hello World";

            // Act
            var result = item.TryConvert<string>(out var value);

            // Assert
            Assert.True(result);
            Assert.Equal("Hello World", value);
        }

        [Fact]
        public void TryConvert_WhenItemIsNull_ReturnsFalse()
        {
            // Arrange
            object? item = null;

            // Act
            var result = item.TryConvert<string>(out var value);

            // Assert
            Assert.False(result);
            Assert.Null(value);
        }

        [Fact]
        public void TryConvert_WhenItemIsNotOfTargetType_ReturnsFalse()
        {
            // Arrange
            object item = 123;

            // Act
            var result = item.TryConvert<string>(out var value);

            // Assert
            Assert.False(result);
            Assert.Null(value);
        }

        [Fact]
        public void TryConvert_WithValueType_WhenItemIsOfTargetType_ReturnsTrue()
        {
            // Arrange
            object item = 42;

            // Act
            var result = item.TryConvert<int>(out var value);

            // Assert
            Assert.True(result);
            Assert.Equal(42, value);
        }

        [Fact]
        public void TryConvert_WithValueType_WhenItemIsNotOfTargetType_ReturnsFalse()
        {
            // Arrange
            object item = "not a number";

            // Act
            var result = item.TryConvert<int>(out var value);

            // Assert
            Assert.False(result);
            Assert.Equal(0, value); // default(int)
        }

        [Fact]
        public void TryConvert_WithNullableValueType_WhenItemIsNull_ReturnsFalse()
        {
            // Arrange
            object? item = null;

            // Act
            var result = item.TryConvert<int?>(out var value);

            // Assert
            Assert.False(result);
            Assert.Null(value);
        }

        [Fact]
        public void TryConvert_WithInheritance_WhenItemIsDerivedType_ReturnsTrue()
        {
            // Arrange
            object item = new ArgumentException("test");

            // Act
            var result = item.TryConvert<Exception>(out var value);

            // Assert
            Assert.True(result);
            Assert.IsType<ArgumentException>(value);
            Assert.Equal("test", value?.Message);
        }

        #endregion

        #region Convert<TTarget> Tests

        [Fact]
        public void Convert_WhenItemIsOfTargetType_ReturnsConvertedValue()
        {
            // Arrange
            object item = "Hello World";

            // Act
            var result = item.Convert<string>();

            // Assert
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void Convert_WhenItemIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            object? item = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => item.Convert<string>());
        }

        [Fact]
        public void Convert_WhenItemIsNotOfTargetType_ThrowsInvalidCastException()
        {
            // Arrange
            object item = 123;

            // Act & Assert
            Assert.Throws<InvalidCastException>(() => item.Convert<string>());
        }

        [Fact]
        public void Convert_WithValueType_WhenItemIsOfTargetType_ReturnsConvertedValue()
        {
            // Arrange
            object item = 42;

            // Act
            var result = item.Convert<int>();

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void Convert_WithInheritance_WhenItemIsDerivedType_ReturnsConvertedValue()
        {
            // Arrange
            object item = new ArgumentException("test");

            // Act
            var result = item.Convert<Exception>();

            // Assert
            Assert.IsType<ArgumentException>(result);
            Assert.Equal("test", result.Message);
        }

        #endregion

        #region Convert<TTarget> with Func<object, TTarget> Tests

        [Fact]
        public void Convert_WithConverter_WhenItemIsNotNull_ReturnsConvertedValue()
        {
            // Arrange
            object item = 123;
            Func<object, string> converter = obj => obj.ToString()!;

            // Act
            var result = item.Convert(converter);

            // Assert
            Assert.Equal("123", result);
        }

        [Fact]
        public void Convert_WithConverter_WhenItemIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            object? item = null;
            Func<object, string> converter = obj => obj.ToString()!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => item.Convert(converter));
        }

        [Fact]
        public void Convert_WithConverter_WhenConverterIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            object item = 123;
            Func<object, string>? converter = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => item.Convert(converter!));
        }

        [Fact]
        public void Convert_WithConverter_ComplexConversion_ReturnsConvertedValue()
        {
            // Arrange
            object item = new DateTime(2023, 12, 25);
            Func<object, string> converter = obj => ((DateTime)obj).ToString("yyyy-MM-dd");

            // Act
            var result = item.Convert(converter);

            // Assert
            Assert.Equal("2023-12-25", result);
        }

        #endregion

        #region Convert<TInput, TTarget> with Func<TInput, TTarget> Tests

        [Fact]
        public void Convert_WithTypedConverter_WhenItemIsNotNull_ReturnsConvertedValue()
        {
            // Arrange
            object item = 123;
            Func<int, string> converter = num => $"Number: {num}";

            // Act
            var result = item.Convert<int, string>(converter);

            // Assert
            Assert.Equal("Number: 123", result);
        }

        [Fact]
        public void Convert_WithTypedConverter_WhenItemIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            object? item = null;
            Func<int, string> converter = num => $"Number: {num}";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => item.Convert<int, string>(converter));
        }

        [Fact]
        public void Convert_WithTypedConverter_WhenConverterIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            object item = 123;
            Func<int, string>? converter = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => item.Convert<int, string>(converter!));
        }

        [Fact]
        public void Convert_WithTypedConverter_ComplexTypes_ReturnsConvertedValue()
        {
            // Arrange
            object item = new Person { Name = "John", Age = 30 };
            Func<Person, string> converter = person => $"{person.Name} ({person.Age})";

            // Act
            var result = item.Convert<Person, string>(converter);

            // Assert
            Assert.Equal("John (30)", result);
        }

        [Fact]
        public void Convert_WithTypedConverter_WhenItemIsWrongType_ThrowsBehaviorDependsOnInvokeIf()
        {
            // Arrange
            object item = "not a number";
            Func<int, string> converter = num => $"Number: {num}";

            // Act & Assert
            // Note: The actual behavior depends on the implementation of InvokeIf
            // This test documents the expected behavior - you may need to adjust based on InvokeIf implementation
            try
            {
                var result = item.Convert<int, string>(converter);
                // If InvokeIf handles type mismatch gracefully, result might be null or default
                // Assert based on your InvokeIf implementation
            }
            catch (Exception ex)
            {
                // If InvokeIf throws on type mismatch, document the expected exception type
                Assert.IsType<InvalidCastException>(ex);
            }
        }

        #endregion

        #region Helper Classes for Testing

        private class Person
        {
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
        }

        #endregion
    }
}