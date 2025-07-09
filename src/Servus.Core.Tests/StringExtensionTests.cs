using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Servus.Core.Tests;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    [DataRow(null, "")]
    [DataRow("", "")]
    [DataRow("   ", "")]
    [DataRow("camelCase", "camel_case")]
    [DataRow("PascalCase", "pascal_case")]
    [DataRow("IOName", "io_name")]
    [DataRow("XMLHttpRequest", "xml_http_request")]
    [DataRow("NameV1", "name_v1")]
    [DataRow("Version1Name", "version_1_name")]
    [DataRow("1Name", "_1_name")]
    [DataRow("123", "_123")]
    [DataRow("Some Name Here", "some_name_here")]
    [DataRow("some_existing_snake_case", "some_existing_snake_case")]
    [DataRow("Some_Mixed Case_Here", "some_mixed_case_here")]
    [DataRow("XMLHttpRequestV2Handler", "xml_http_request_v2_handler")]
    [DataRow("A", "a")]
    [DataRow("HELLO", "hello")]
    [DataRow("already_snake_case", "already_snake_case")]
    [DataRow("Multiple   Spaces", "multiple_spaces")]
    [DataRow("  SomeValue  ", "some_value")]
    [DataRow("Test123Value456End", "test_123_value_456_end")]
    public void ToSnakeCase_VariousInputs_ReturnsExpectedResult(string input, string expected)
    {
        // Act
        var result = input.ToSnakeCase();

        // Assert
        Assert.AreEqual(expected, result);
    }
}