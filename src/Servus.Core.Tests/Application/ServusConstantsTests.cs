using Xunit;

namespace Servus.Core.Tests.Application;

public class ServusConstantsTests
{
    [Fact]
    public void LogoTests()
    {
        Assert.Contains("servus!", Core.Application.Servus.Logo);
    }

    [Fact]
    public void LogoSmallTests()
    {
        Assert.Contains("servus!", Core.Application.Servus.LogoSmall);
    }

    [Fact]
    public void LogoTinyTests()
    {
        Assert.Contains("servus!", Core.Application.Servus.LogoTiny);
    }
}