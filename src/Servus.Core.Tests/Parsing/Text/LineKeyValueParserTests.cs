using Servus.Core.Parsing.Text;
using Xunit;
using System;

namespace Servus.Core.Tests.Parsing.Text;

public class LineKeyValueParserTests
{
    private LineKeyValueParser _parser = null!;

    public LineKeyValueParserTests()
    {
        _parser = new LineKeyValueParser('=');
    }

    [Theory]
    [InlineData("h=z3FaW23KmdHWIyBA99Cztqu7MHc=", "h", "z3FaW23KmdHWIyBA99Cztqu7MHc=")]
    [InlineData("sl=25", "sl", "25")]
    public void ParseLineTest(string input, string key, string value)
    {
        var parsed = _parser.ParseLine(input);
        Assert.Equal(key, parsed.Key);
        Assert.Equal(value, parsed.Value);
    }

    [Fact]
    public void ParseMultiline()
    {
        var lines = "h=z3FaW23KmdHWIyBA99Cztqu7MHc=" + Environment.NewLine +
                    "t=2019-09-15T09:13:15Z0817" + Environment.NewLine +
                    "otp=cccccclibubjhkuttefctkgejjgerdjfihbkhtddivju" + Environment.NewLine +
                    "nonce=aef3a7835277a28da831005c2ae3b919e2076a62" + Environment.NewLine +
                    "sl=25" + Environment.NewLine +
                    "status=OK" + Environment.NewLine;

        var results = _parser.Parse(lines);
        var r = results.GetEnumerator();
        r.MoveNext();
        Assert.Equal("h", r.Current.Key);
        Assert.Equal("z3FaW23KmdHWIyBA99Cztqu7MHc=", r.Current.Value);

        r.MoveNext();
        Assert.Equal("t", r.Current.Key);
        Assert.Equal("2019-09-15T09:13:15Z0817", r.Current.Value);

        r.MoveNext();
        Assert.Equal("otp", r.Current.Key);
        Assert.Equal("cccccclibubjhkuttefctkgejjgerdjfihbkhtddivju", r.Current.Value);

        r.MoveNext();
        Assert.Equal("nonce", r.Current.Key);
        Assert.Equal("aef3a7835277a28da831005c2ae3b919e2076a62", r.Current.Value);

        r.MoveNext();
        Assert.Equal("sl", r.Current.Key);
        Assert.Equal("25", r.Current.Value);

        r.MoveNext();
        Assert.Equal("status", r.Current.Key);
        Assert.Equal("OK", r.Current.Value);
    }
}