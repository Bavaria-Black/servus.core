using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Application.Console;

namespace Servus.Core.Tests.Application.Console;

[TestClass]
public class ConsoleRedirectorTests
{
    [TestMethod]
    public void RedirectTests()
    {
        using var redirector = new ConsoleRedirector();
        
        Assert.IsEmpty(redirector.ToString());
        System.Console.Write("Leberkas");
        Assert.AreEqual("Leberkas", redirector.ToString());
    }    
}