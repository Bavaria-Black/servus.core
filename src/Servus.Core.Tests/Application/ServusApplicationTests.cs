using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Application;

namespace Servus.Core.Tests.Application;

[TestClass]
public class ServusApplicationTests
{
    [TestMethod]
    public void EnvironmentVariableTests()
    {
        Environment.SetEnvironmentVariable("SERVUS_UNITTEST_TEST_VALUE", "LEBERKAS");
        Assert.AreEqual("LEBERKAS", ServusApplication.GetEnvironmentVariable("UNITTEST_TEST_VALUE"));
        Assert.IsTrue(ServusApplication.IsEnvironmentVariableSetTo("UNITTEST_TEST_VALUE", "LEBERKAS"));
    }
}