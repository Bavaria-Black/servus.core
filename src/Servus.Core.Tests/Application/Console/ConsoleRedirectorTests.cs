using Servus.Core.Application.Console;
using Xunit;

namespace Servus.Core.Tests.Application.Console;

[CollectionDefinition("ConsoleTests", DisableParallelization = true)]
public class SerialTestCollection;

[Collection("ConsoleTests")]
public class ConsoleRedirectorTests
{
    [Fact]
    public void RedirectTests()
    {
        using var redirector = new ConsoleRedirector();
        
        Assert.Empty(redirector.ToString());
        // ReSharper disable once Xunit.XunitTestWithConsoleOutput
        System.Console.Write("Leberkas");
        Assert.Equal("Leberkas", redirector.ToString());
    }    
}