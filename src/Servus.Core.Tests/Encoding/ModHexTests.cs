using Servus.Core.Encoding;
using Xunit;

namespace Servus.Core.Tests.Encoding;

public class ModHexTests
{
    [Fact]
    public void Encode()
    {
        var endcoding = new ModHexEncoding();
        var bytes = endcoding.GetBytes("test");
        var result = System.Text.Encoding.ASCII.GetString(bytes);

        Assert.Equal("ifhgieif", result);
    }

    [Fact]
    public void Decode()
    {
        var endcoding = new ModHexEncoding();
        var bytes = System.Text.Encoding.ASCII.GetBytes("ifhgieif");
        var result = endcoding.GetString(bytes);

        Assert.Equal("test", result);
    }
}