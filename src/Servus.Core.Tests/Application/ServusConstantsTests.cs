
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Servus.Core.Tests.Application;

[TestClass]
public class ServusConstantsTests
{
    [TestMethod]
    public void LogoTests()
    {
        Assert.IsTrue(Core.Application.Servus.Logo.Contains("servus!"));
    }
    [TestMethod]
    public void LogoSmallTests()
    {
        Assert.IsTrue(Core.Application.Servus.LogoSmall.Contains("servus!"));
    }
    [TestMethod]
    public void LogoTinyTests()
    {
        Assert.IsTrue(Core.Application.Servus.LogoTiny.Contains("servus!"));
    }
}