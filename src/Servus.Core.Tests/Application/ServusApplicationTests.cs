using System;
using Servus.Core.Application;
using Xunit;

namespace Servus.Core.Tests.Application;

public class ServusApplicationTests
{
    [Fact]
    public void EnvironmentVariableTests()
    {
        Environment.SetEnvironmentVariable("SERVUS_UNITTEST_TEST_VALUE", "LEBERKAS");
        Assert.Equal("LEBERKAS", ServusApplication.GetEnvironmentVariable("UNITTEST_TEST_VALUE"));
        Assert.True(ServusApplication.IsEnvironmentVariableSetTo("UNITTEST_TEST_VALUE", "LEBERKAS"));
    }
}