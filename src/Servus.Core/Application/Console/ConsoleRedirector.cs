using System.IO;

namespace Servus.Core.Application.Console;

internal class ConsoleRedirector : IDisposable
{
    private readonly StringWriter _consoleOutput = new();
    private readonly TextWriter _originalConsoleOutput;
    
    public ConsoleRedirector()
    {
        _originalConsoleOutput = System.Console.Out;
        System.Console.SetOut(_consoleOutput);
    }
    public void Dispose()
    {
        System.Console.SetOut(_originalConsoleOutput);
        System.Console.Write(this.ToString());
        _consoleOutput.Dispose();
    }
    public override string ToString()
    {
        return _consoleOutput.ToString();
    }
}