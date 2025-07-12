using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Conversion;

namespace Servus.Core.Tests.Conversion
{
    [TestClass]
    public class StringConverterCollectionTests
    {
        private StringConverterCollection _collection;

        [TestInitialize]
        public void Setup()
        {
            _collection = new StringConverterCollection();
        }

        #region Register Tests

        [TestMethod]
        public void Register_ValidConverter_ConverterIsRegistered()
        {
            // Arrange
            var converter = new StringValueConverter();

            // Act
            _collection.Register(converter);
            var result = _collection.Convert<string>("test");

            // Assert
            Assert.AreEqual("test", result);
        }

        [TestMethod]
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
            Assert.AreEqual("test", result); // Should use first converter
        }

        #endregion

        #region Convert Generic Tests

        [TestMethod]
        [DataRow("42", 42)]
        [DataRow("-123", -123)]
        [DataRow("0", 0)]
        [DataRow("2147483647", 2147483647)]
        public void Convert_Integer_ReturnsCorrectValue(string input, int expected)
        {
            // Arrange
            _collection.Register(new IntegerValueConverter());

            // Act
            var result = _collection.Convert<int>(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("3.14", 3.14)]
        [DataRow("-2.5", -2.5)]
        [DataRow("0.0", 0.0)]
        [DataRow("123.456789", 123.456789)]
        public void Convert_Double_ReturnsCorrectValue(string input, double expected)
        {
            // Arrange
            _collection.Register(new DoubleValueConverter());

            // Act
            var result = _collection.Convert<double>(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("3.14", 3.14f)]
        [DataRow("-2.5", -2.5f)]
        [DataRow("0.0", 0.0f)]
        [DataRow("123.45", 123.45f)]
        public void Convert_Float_ReturnsCorrectValue(string input, float expected)
        {
            // Arrange
            _collection.Register(new FloatValueConverter());

            // Act
            var result = _collection.Convert<float>(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("true", true)]
        [DataRow("false", false)]
        [DataRow("True", true)]
        [DataRow("False", false)]
        [DataRow("TRUE", true)]
        [DataRow("FALSE", false)]
        [DataRow("y", true)]
        [DataRow("n", false)]
        [DataRow("ja", true)]
        [DataRow("na", false)]
        [DataRow("JA", true)]
        [DataRow("NA", false)]
        public void Convert_Bool_ReturnsCorrectValue(string input, bool expected)
        {
            // Arrange
            _collection.Register(new BoolValueConverter(["y", "ja"], ["n", "na"]));

            // Act
            var result = _collection.Convert<bool>(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("hello")]
        [DataRow("")]
        [DataRow("  spaces  ")]
        [DataRow("special@chars#123")]
        public void Convert_String_ReturnsInputValue(string input)
        {
            // Arrange
            _collection.Register(new StringValueConverter());

            // Act
            var result = _collection.Convert<string>(input);

            // Assert
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void Convert_UnregisteredType_ReturnsNull()
        {
            // Act
            var result = _collection.Convert<DateTime>("2023-01-01");

            // Assert
            Assert.IsNull(result);
        }

        #endregion
        
        #region Convert KeyValue Tests

        [TestMethod]
        public void Convert_ByType_ReturnsCorrectValue()
        {
            
            // Arrange
            var expected = new KeyValue("42", "45");
            RegisterAllBasicConverters();

            // Act
            var result = _collection.Convert<KeyValue>("42;45");

            // Assert
            Assert.AreEqual(expected, result);
        }
        
        #endregion

        #region Convert Type Tests

        [TestMethod]
        [DataRow(typeof(int), "42", 42)]
        [DataRow(typeof(double), "3.14", 3.14)]
        [DataRow(typeof(bool), "true", true)]
        [DataRow(typeof(string), "test", "test")]
        public void Convert_ByType_ReturnsCorrectValue(Type targetType, string input, object expected)
        {
            // Arrange
            RegisterAllBasicConverters();

            // Act
            var result = _collection.Convert(targetType, input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Convert_ByType_UnregisteredType_ReturnsNull()
        {
            // Act
            var result = _collection.Convert(typeof(DateTime), "2023-01-01");

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Exception Handling Tests

        [TestMethod]
        [DataRow("not_a_number")]
        [DataRow("abc")]
        [DataRow("12.34")] // Invalid for int
        [DataRow("")]
        public void Convert_InvalidInteger_ReturnsNull(string invalidInput)
        {
            // Arrange
            _collection.Register(new IntegerValueConverter());

            // Act
            var result = _collection.Convert<int>(invalidInput);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [DataRow("not_a_bool")]
        [DataRow("yes")]
        [DataRow("1")]
        [DataRow("0")]
        public void Convert_InvalidBool_ReturnsNull(string invalidInput)
        {
            // Arrange
            _collection.Register(new BoolValueConverter());

            // Act
            var result = _collection.Convert<bool>(invalidInput);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Convert_WithCustomExceptionHandler_ReturnsHandlerResult()
        {
            // Arrange
            _collection.Register(new IntegerValueConverter());
            _collection.RegisterExceptionHandler(ex => -1); // Return -1 on error

            // Act
            var result = _collection.Convert<int>("invalid");

            // Assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void Convert_WithCustomExceptionHandler_ReceivesCorrectException()
        {
            // Arrange
            Exception capturedException = null;
            _collection.Register(new IntegerValueConverter());
            _collection.RegisterExceptionHandler(ex => { capturedException = ex; return null; });

            // Act
            _collection.Convert<int>("invalid");

            // Assert
            Assert.IsNotNull(capturedException);
            Assert.IsInstanceOfType(capturedException, typeof(FormatException));
        }

        [TestMethod]
        public void RegisterExceptionHandler_MultipleRegistrations_LastHandlerIsUsed()
        {
            // Arrange
            _collection.Register(new IntegerValueConverter());
            _collection.RegisterExceptionHandler(ex => "first");
            _collection.RegisterExceptionHandler(ex => "second");

            // Act
            var result = _collection.Convert<int>("invalid");

            // Assert
            Assert.AreEqual("second", result);
        }

        #endregion

        #region Individual Converter Tests

        [TestMethod]
        public void StringValueConverter_OutputType_ReturnsStringType()
        {
            // Arrange
            var converter = new StringValueConverter();

            // Act & Assert
            Assert.AreEqual(typeof(string), converter.OutputType);
        }

        [TestMethod]
        public void IntegerValueConverter_OutputType_ReturnsIntType()
        {
            // Arrange
            var converter = new IntegerValueConverter();

            // Act & Assert
            Assert.AreEqual(typeof(int), converter.OutputType);
        }

        [TestMethod]
        public void DoubleValueConverter_OutputType_ReturnsDoubleType()
        {
            // Arrange
            var converter = new DoubleValueConverter();

            // Act & Assert
            Assert.AreEqual(typeof(double), converter.OutputType);
        }

        [TestMethod]
        public void FloatValueConverter_OutputType_ReturnsFloatType()
        {
            // Arrange
            var converter = new FloatValueConverter();

            // Act & Assert
            Assert.AreEqual(typeof(float), converter.OutputType);
        }

        [TestMethod]
        public void BoolValueConverter_OutputType_ReturnsBoolType()
        {
            // Arrange
            var converter = new BoolValueConverter();

            // Act & Assert
            Assert.AreEqual(typeof(bool), converter.OutputType);
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void RegisterAllConverters_MultipleConversions_WorkCorrectly()
        {
            // Arrange
            RegisterAllBasicConverters();

            // Act & Assert
            Assert.AreEqual("test", _collection.Convert<string>("test"));
            Assert.AreEqual(42, _collection.Convert<int>("42"));
            Assert.AreEqual(3.14, _collection.Convert<double>("3.14"));
            Assert.AreEqual(2.5f, _collection.Convert<float>("2.5"));
            Assert.AreEqual(true, _collection.Convert<bool>("true"));
        }

        [TestMethod]
        public void Convert_MixedValidAndInvalidInputs_HandlesCorrectly()
        {
            // Arrange
            RegisterAllBasicConverters();

            // Act & Assert
            Assert.AreEqual(42, _collection.Convert<int>("42")); // Valid
            Assert.IsNull(_collection.Convert<int>("invalid")); // Invalid
            Assert.AreEqual(true, _collection.Convert<bool>("true")); // Valid
            Assert.IsNull(_collection.Convert<bool>("maybe")); // Invalid
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