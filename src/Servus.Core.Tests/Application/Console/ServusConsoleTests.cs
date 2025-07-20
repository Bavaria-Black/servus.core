using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Application.Console;

namespace Servus.Core.Tests.Application.Console;

[TestClass]
public class ServusConsoleTests
{
    [TestMethod]
    public void WriteColoredTests()
    {
        using var redirector = new ConsoleRedirector();

        Assert.IsEmpty(redirector.ToString());
        ServusConsole.WriteColored("Leberkas", ConsoleColor.Black);
        Assert.AreEqual("Leberkas", redirector.ToString());
    }

    [TestMethod]
    public void WriteLineColoredTests()
    {
        using var redirector = new ConsoleRedirector();

        Assert.IsEmpty(redirector.ToString());
        ServusConsole.WriteLineColored("Leberkas", ConsoleColor.Black);
        Assert.AreEqual("Leberkas" + Environment.NewLine, redirector.ToString());
    }

    [TestMethod]
    public void WriteKeyValueTests()
    {
        using var redirector = new ConsoleRedirector();

        Assert.IsEmpty(redirector.ToString());
        var kvp = new KeyValuePair<string, string>("Key", "Value");
        ServusConsole.PrintKeyValue(kvp);
        Assert.AreEqual(" [Key]           => Value" + Environment.NewLine, redirector.ToString());
    }

    [TestMethod]
    public void WriteKeyValueNonDefaultTests()
    {
        using var redirector = new ConsoleRedirector();

        Assert.IsEmpty(redirector.ToString());
        var kvp = new KeyValuePair<int, int>(1, 555);
        ServusConsole.PrintKeyValue(kvp);
        Assert.AreEqual(" [1]             => 555" + Environment.NewLine, redirector.ToString());
    }

    [TestMethod]
    public void PrintLineTests()
    {
        using var redirector = new ConsoleRedirector();
        Assert.IsEmpty(redirector.ToString());
        
        ServusConsole.PrintLine(10, '_');
        Assert.AreEqual("__________" + Environment.NewLine, redirector.ToString());
    }
}