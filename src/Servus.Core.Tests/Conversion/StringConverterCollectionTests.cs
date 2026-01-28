using System;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Servus.Core.Conversion;

namespace Servus.Core.Tests.Conversion
{
    public class StringConverterCollectionTests
    {
        private StringConverterCollection _collection = new ();

        public StringConverterCollectionTests()
        {
            _collection = new StringConverterCollection();
        }

        #region Register Tests

        [Fact]
        public void Register_ValidConverter_ConverterIsRegistered()
        {
            // Arrange
            var converter = new StringValueConverter();

            // Act
            _collection.Register(converter);
            var result = _collection.Convert<string>("test");

            // Assert
            Assert.Equal("test", result);
        }

        [Fact]
        public void Register_DuplicateType_FirstConverterIsKept()
        {
            // Arrange
            var converter1 = new StringValueConverter();
            var converter2 = new TestStringConverter("converted");

            // Act
            _collection.Register(converter1);
            _collection.Register(converter2); // Should not replace

            var result = _collection.Convert<string>("test");

            // Assert
            Assert.Equal("test", result); // Should use first converter
        }

        #endregion

        #region Convert Generic Tests

        [Theory]
        [InlineData("42", 42)]
        [InlineData("-123", -123)]
        [InlineData("0", 0)]
        [InlineData("2147483647", 2147483647)]
        public void Convert_Integer_ReturnsCorrectValue(string input, int expected)
        {
            // Arrange
            _collection.Register(new IntegerValueConverter());

            // Act
            var result = _collection.Convert<int>(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("3.14", 3.14)]
        [InlineData("-2.5", -2.5)]
        [InlineData("0.0", 0.0)]
        [InlineData("123.456789", 123.456789)]
        public void Convert_Double_ReturnsCorrectValue(string input, double expected)
        {
            // Arrange
            _collection.Register(new DoubleValueConverter());

            // Act
            var result = _collection.Convert<double>(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("3.14", 3.14f)]
        [InlineData("-2.5", -2.5f)]
        [InlineData("0.0", 0.0f)]
        [InlineData("123.45", 123.45f)]
        public void Convert_Float_ReturnsCorrectValue(string input, float expected)
        {
            // Arrange
            _collection.Register(new FloatValueConverter());

            // Act
            var result = _collection.Convert<float>(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("True", true)]
        [InlineData("False", false)]
        [InlineData("TRUE", true)]
        [InlineData("FALSE", false)]
        [InlineData("y", true)]
        [InlineData("n", false)]
        [InlineData("ja", true)]
        [InlineData("na", false)]
        [InlineData("JA", true)]
        [InlineData("NA", false)]
        public void Convert_Bool_ReturnsCorrectValue(string input, bool expected)
        {
            // Arrange
            _collection.Register(new BoolValueConverter(["y", "ja"], ["n", "na"]));

            // Act
            var result = _collection.Convert<bool>(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("hello")]
        [InlineData("")]
        [InlineData("  spaces  ")]
        [InlineData("special@chars#123")]
        public void Convert_String_ReturnsInputValue(string input)
        {
            // Arrange
            _collection.Register(new StringValueConverter());

            // Act
            var result = _collection.Convert<string>(input);

            // Assert
            Assert.Equal(input, result);
        }

        [Fact]
        public void Convert_UnregisteredType_ReturnsNull()
        {
            // Act
            var result = _collection.Convert<DateTime>("2023-01-01");

            // Assert
            Assert.Null(result);
        }

        #endregion
        
        #region Convert KeyValue Tests

        [Fact]
        public void Convert_ByGenericType_ReturnsCorrectValue()
        {
            
            // Arrange
            var expected = new KeyValue("42", "45");
            RegisterAllBasicConverters();

            // Act
            var result = _collection.Convert<KeyValue>("42;45");

            // Assert
            Assert.Equal(expected, result);
        }
        
        #endregion

        #region Convert Type Tests

        [Theory]
        [InlineData(typeof(int), "42", 42)]
        [InlineData(typeof(double), "3.14", 3.14)]
        [InlineData(typeof(bool), "true", true)]
        [InlineData(typeof(string), "test", "test")]
        public void Convert_ByType_ReturnsCorrectValue(Type targetType, string input, object expected)
        {
            // Arrange
            RegisterAllBasicConverters();

            // Act
            var result = _collection.Convert(targetType, input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Convert_ByType_UnregisteredType_ReturnsNull()
        {
            // Act
            var result = _collection.Convert(typeof(DateTime), "2023-01-01");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Exception Handling Tests

        [Theory]
        [InlineData("not_a_number")]
        [InlineData("abc")]
        [InlineData("12.34")] // Invalid for int
        [InlineData("")]
        public void Convert_InvalidInteger_ReturnsNull(string invalidInput)
        {
            // Arrange
            _collection.Register(new IntegerValueConverter());

            // Act
            var result = _collection.Convert<int>(invalidInput);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("not_a_bool")]
        [InlineData("yes")]
        [InlineData("1")]
        [InlineData("0")]
        public void Convert_InvalidBool_ReturnsNull(string invalidInput)
        {
            // Arrange
            _collection.Register(new BoolValueConverter());

            // Act
            var result = _collection.Convert<bool>(invalidInput);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Convert_WithCustomExceptionHandler_ReturnsHandlerResult()
        {
            // Arrange
            _collection.Register(new IntegerValueConverter());
            _collection.RegisterExceptionHandler(ex => -1); // Return -1 on error

            // Act
            var result = _collection.Convert<int>("invalid");

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void Convert_WithCustomExceptionHandler_ReceivesCorrectException()
        {
            // Arrange
            Exception capturedException = null!;
            _collection.Register(new IntegerValueConverter());
            _collection.RegisterExceptionHandler(ex => { capturedException = ex; return null; });

            // Act
            _collection.Convert<int>("invalid");

            // Assert
            Assert.NotNull(capturedException);
            Assert.IsType<FormatException>(capturedException);
        }

        [Fact]
        public void RegisterExceptionHandler_MultipleRegistrations_LastHandlerIsUsed()
        {
            // Arrange
            _collection.Register(new IntegerValueConverter());
            _collection.RegisterExceptionHandler(ex => "first");
            _collection.RegisterExceptionHandler(ex => "second");

            // Act
            var result = _collection.Convert<int>("invalid");

            // Assert
            Assert.Equal("second", result);
        }

        #endregion

        #region Individual Converter Tests

        [Fact]
        public void StringValueConverter_OutputType_ReturnsStringType()
        {
            // Arrange
            var converter = new StringValueConverter();

            // Act & Assert
            Assert.Equal(typeof(string), converter.OutputType);
        }

        [Fact]
        public void IntegerValueConverter_OutputType_ReturnsIntType()
        {
            // Arrange
            var converter = new IntegerValueConverter();

            // Act & Assert
            Assert.Equal(typeof(int), converter.OutputType);
        }

        [Fact]
        public void DoubleValueConverter_OutputType_ReturnsDoubleType()
        {
            // Arrange
            var converter = new DoubleValueConverter();

            // Act & Assert
            Assert.Equal(typeof(double), converter.OutputType);
        }

        [Fact]
        public void FloatValueConverter_OutputType_ReturnsFloatType()
        {
            // Arrange
            var converter = new FloatValueConverter();

            // Act & Assert
            Assert.Equal(typeof(float), converter.OutputType);
        }

        [Fact]
        public void BoolValueConverter_OutputType_ReturnsBoolType()
        {
            // Arrange
            var converter = new BoolValueConverter();

            // Act & Assert
            Assert.Equal(typeof(bool), converter.OutputType);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void RegisterAllConverters_MultipleConversions_WorkCorrectly()
        {
            // Arrange
            RegisterAllBasicConverters();

            // Act & Assert
            Assert.Equal("test", _collection.Convert<string>("test"));
            Assert.Equal(42, _collection.Convert<int>("42"));
            Assert.Equal(3.14, _collection.Convert<double>("3.14"));
            Assert.Equal(2.5f, _collection.Convert<float>("2.5"));
            Assert.Equal(true, _collection.Convert<bool>("true"));
        }

        [Fact]
        public void Convert_MixedValidAndInvalidInputs_HandlesCorrectly()
        {
            // Arrange
            RegisterAllBasicConverters();

            // Act & Assert
            Assert.Equal(42, _collection.Convert<int>("42")); // Valid
            Assert.Null(_collection.Convert<int>("invalid")); // Invalid
            Assert.Equal(true, _collection.Convert<bool>("true")); // Valid
            Assert.Null(_collection.Convert<bool>("maybe")); // Invalid
        }

        #endregion

        #region Helper Methods

        private void RegisterAllBasicConverters()
        {
            _collection.Register(new StringValueConverter());
            _collection.Register(new IntegerValueConverter());
            _collection.Register(new DoubleValueConverter());
            _collection.Register(new FloatValueConverter());
            _collection.Register(new BoolValueConverter(["y", "ja"], ["n", "na"]));
            _collection.Register(new KeyValueConverter());
        }

        #endregion

        #region Test Helper Classes

        private class TestStringConverter : IStringValueConverter
        {
            private readonly string _result;

            public TestStringConverter(string result)
            {
                _result = result;
            }

            public Type OutputType => typeof(string);
            public object? Convert(string value) => _result;
        }
        
        private record KeyValue(string Key, string Value);
        
        private class KeyValueConverter : IStringValueConverter
        {
            public Type OutputType => typeof(KeyValue);
            public object? Convert(string value)
            {
                var split =value.Split(";");
                return new KeyValue(split[0], split[1]);
            }
        }

        #endregion
    }
}