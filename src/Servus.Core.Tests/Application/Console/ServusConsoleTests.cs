using System;
using System.Collections.Generic;
using Servus.Core.Application.Console;
using Xunit;

namespace Servus.Core.Tests.Application.Console;

public class ServusConsoleTests
{
    [Fact]
    public void WriteColoredTests()
    {
        using var redirector = new ConsoleRedirector();

        Assert.Empty(redirector.ToString());
        ServusConsole.WriteColored("Leberkas", ConsoleColor.Black);
        Assert.Equal("Leberkas", redirector.ToString());
    }

    [Fact]
    public void WriteLineColoredTests()
    {
        using var redirector = new ConsoleRedirector();

        Assert.Empty(redirector.ToString());
        ServusConsole.WriteLineColored("Leberkas", ConsoleColor.Black);
        Assert.Equal("Leberkas" + Environment.NewLine, redirector.ToString());
    }

    [Fact]
    public void WriteKeyValueTests()
    {
        using var redirector = new ConsoleRedirector();

        Assert.Empty(redirector.ToString());
        var kvp = new KeyValuePair<string, string>("Key", "Value");
        ServusConsole.PrintKeyValue(kvp);
        Assert.Equal(" [Key]           => Value" + Environment.NewLine, redirector.ToString());
    }

    [Fact]
    public void WriteKeyValueNonDefaultTests()
    {
        using var redirector = new ConsoleRedirector();

        Assert.Empty(redirector.ToString());
        var kvp = new KeyValuePair<int, int>(1, 555);
        ServusConsole.PrintKeyValue(kvp);
        Assert.Equal(" [1]             => 555" + Environment.NewLine, redirector.ToString());
    }

    [Fact]
    public void PrintLineTests()
    {
        using var redirector = new ConsoleRedirector();
        Assert.Empty(redirector.ToString());
        
        ServusConsole.PrintLine(10, '_');
        Assert.Equal("__________" + Environment.NewLine, redirector.ToString());
    }
}