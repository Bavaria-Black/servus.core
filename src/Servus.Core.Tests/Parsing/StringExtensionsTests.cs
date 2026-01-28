using System.Linq;
using Servus.Core.Parsing;
using Xunit;

namespace Servus.Core.Tests.Parsing;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, 0)]
    [InlineData("", 0)]
    [InlineData("         ", 1)]
    [InlineData(" ", 1)]
    [InlineData(" \r\n", 1)]
    [InlineData(" \r\n ", 2)]
    [InlineData("test\rtest", 2)]
    [InlineData(" This\n is\r a\r\n Test!", 4)]
    [InlineData(" 1\n\r3", 3)]
    public void GetLinesReturnsExpectedNumberOfLines(string text, int lines)
    {
        var numberOfLines = text.GetLines().Count();
        Assert.Equal(lines, numberOfLines);
    }
}