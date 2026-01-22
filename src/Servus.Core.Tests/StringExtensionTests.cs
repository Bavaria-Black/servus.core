using Servus.Core.Text;
using Xunit;

namespace Servus.Core.Tests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("camelCase", "camel_case")]
    [InlineData("PascalCase", "pascal_case")]
    [InlineData("IOName", "io_name")]
    [InlineData("XMLHttpRequest", "xml_http_request")]
    [InlineData("NameV1", "name_v1")]
    [InlineData("Version1Name", "version_1_name")]
    [InlineData("1Name", "_1_name")]
    [InlineData("123", "_123")]
    [InlineData("Some Name Here", "some_name_here")]
    [InlineData("some_existing_snake_case", "some_existing_snake_case")]
    [InlineData("Some_Mixed Case_Here", "some_mixed_case_here")]
    [InlineData("XMLHttpRequestV2Handler", "xml_http_request_v2_handler")]
    [InlineData("A", "a")]
    [InlineData("HELLO", "hello")]
    [InlineData("already_snake_case", "already_snake_case")]
    [InlineData("Multiple   Spaces", "multiple_spaces")]
    [InlineData("  SomeValue  ", "some_value")]
    [InlineData("Test123Value456End", "test_123_value_456_end")]
    public void ToSnakeCase_VariousInputs_ReturnsExpectedResult(string input, string expected)
    {
        // Act
        var result = input.ToSnakeCase();

        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("camelCase", "camel.case")]
    [InlineData("PascalCase", "pascal.case")]
    [InlineData("IOName", "io.name")]
    [InlineData("XMLHttpRequest", "xml.http.request")]
    [InlineData("NameV1", "name.v1")]
    [InlineData("Version1Name", "version.1.name")]
    [InlineData("1Name", ".1.name")]
    [InlineData("123", ".123")]
    [InlineData("Some Name Here", "some.name.here")]
    [InlineData("some.existing.snake.case", "some.existing.snake.case")]
    [InlineData("Some.Mixed Case.Here", "some.mixed.case.here")]
    [InlineData("XMLHttpRequestV2Handler", "xml.http.request.v2.handler")]
    [InlineData("A", "a")]
    [InlineData("HELLO", "hello")]
    [InlineData("Multiple   Spaces", "multiple.spaces")]
    [InlineData("  SomeValue  ", "some.value")]
    [InlineData("Test123Value456End", "test.123.value.456.end")]
    public void ToDotCase_VariousInputs_ReturnsExpectedResult(string input, string expected)
    {
        // Act
        var result = input.ToDotCase();

        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("camelCase", "camel-case")]
    [InlineData("PascalCase", "pascal-case")]
    [InlineData("IOName", "io-name")]
    [InlineData("XMLHttpRequest", "xml-http-request")]
    [InlineData("NameV1", "name-v1")]
    [InlineData("Version1Name", "version-1-name")]
    [InlineData("1Name", "-1-name")]
    [InlineData("123", "-123")]
    [InlineData("Some Name Here", "some-name-here")]
    [InlineData("some-existing-snake-case", "some-existing-snake-case")]
    [InlineData("Some-Mixed Case-Here", "some-mixed-case-here")]
    [InlineData("XMLHttpRequestV2Handler", "xml-http-request-v2-handler")]
    [InlineData("A", "a")]
    [InlineData("HELLO", "hello")]
    [InlineData("Multiple   Spaces", "multiple-spaces")]
    [InlineData("  SomeValue  ", "some-value")]
    [InlineData("Test123Value456End", "test-123-value-456-end")]
    public void ToKebabCase_VariousInputs_ReturnsExpectedResult(string input, string expected)
    {
        // Act
        var result = input.ToKebabCase();

        // Assert
        Assert.Equal(expected, result);
    }
}