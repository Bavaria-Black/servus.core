using System.IO;

namespace Servus.Core.Application.Console;


/// <summary>
/// Temporarily redirects console output to a string buffer, allowing console output to be captured and restored.
/// Implements IDisposable to ensure proper cleanup and restoration of original console output.
/// </summary>
/// <remarks>
/// This class captures all Console.Write and Console.WriteLine operations during its lifetime.
/// When disposed, it restores the original console output and writes the captured content to the console.
/// </remarks>
internal class ConsoleRedirector : IDisposable
{
    private readonly StringWriter _consoleOutput = new();
    private readonly TextWriter _originalConsoleOutput;
    
    /// <summary>
    /// Initializes a new instance of the ConsoleRedirector class and begins capturing console output.
    /// </summary>
    /// <remarks>
    /// Immediately redirects Console.Out to an internal StringWriter to begin capturing output.
    /// </remarks>
    public ConsoleRedirector()
    {
        _originalConsoleOutput = System.Console.Out;
        System.Console.SetOut(_consoleOutput);
    }
    
    /// <summary>
    /// Restores the original console output, writes the captured content to the console, and releases resources.
    /// </summary>
    /// <remarks>
    /// This method restores Console.Out to its original state, outputs all captured content to the console,
    /// and properly disposes of the internal StringWriter.
    /// </remarks>
    public void Dispose()
    {
        System.Console.SetOut(_originalConsoleOutput);
        System.Console.Write(this.ToString());
        _consoleOutput.Dispose();
    }
    
    /// <summary>
    /// Returns the captured console output as a string.
    /// </summary>
    /// <returns>A string containing all console output that was captured during the lifetime of this redirector.</returns>
    public override string ToString()
    {
        return _consoleOutput.ToString();
    }
}