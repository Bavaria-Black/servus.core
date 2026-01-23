using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Servus.Core.Application.Console;
using Xunit;

namespace Servus.Core.Tests.Application.Console;

[Collection("ConsoleTests")]
public class ProcessOutRedirectorTests
{
    private static Process CreateTestProcess(string arguments = "")
    {
        // Use a simple command that works on both Windows and Unix
        var isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
        var fileName = isWindows ? "cmd.exe" : "/bin/echo";
        var args = isWindows ? $"/c echo {arguments}" : arguments;

        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
    }

    private static Process CreateLongRunningProcess()
    {
        var isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            
        if (isWindows)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c ping 127.0.0.1 -n 2",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
        }
        else
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sleep",
                    Arguments = "5",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
        }
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidProcess_InitializesCorrectly()
    {
        // Arrange
        using var process = CreateTestProcess();

        // Act
        using var redirector = new ProcessOutRedirector(process);

        // Assert
        Assert.False(process.StartInfo.UseShellExecute);
        Assert.True(process.StartInfo.RedirectStandardOutput);
        Assert.True(process.StartInfo.RedirectStandardError);
    }

    [Fact]
    public void Constructor_WithNullProcess_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ProcessOutRedirector(null!));
    }

    #endregion

    #region StartRedirection Tests

    [Fact]
    public void StartRedirection_WhenAlreadyActive_ThrowsInvalidOperationException()
    {
        // Arrange
        using var process = CreateLongRunningProcess();
        using var redirector = new ProcessOutRedirector(process);
            
        process.Start();
            
        // Start redirection in background
        redirector.StartRedirection();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => redirector.StartRedirection());
            
        // Cleanup
        redirector.StopRedirection();
        process.Kill();
    }
    
    [Fact]
    public void StartRedirection_WhenProcessHasExited_ThrowsInvalidOperationException()
    {
        // Arrange
        using var process = CreateTestProcess("test");
        using var redirector = new ProcessOutRedirector(process);
            
        process.Start();
        process.WaitForExit(5000); // Wait for process to exit

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => redirector.StartRedirection());
    }

    #endregion

    #region StartRedirectionAsync Tests

    [Fact]
    public async Task StartRedirectionAsync_WithValidProcess_CompletesSuccessfully()
    {
        // Arrange
        using var process = CreateTestProcess("Hello Async");
        using var redirector = new ProcessOutRedirector(process);
            
        process.Start();

        // Act
        await redirector.StartRedirectionAsync();
        
        process.Kill();

        await Task.Delay(100);
        
        // Assert
        Assert.True(process.HasExited);
    }

    [Fact]
    public async Task StartRedirectionAsync_WhenCancelled_ThrowsInvalidOperationException()
    {
        // Arrange
        using var process = CreateTestProcess("test");
        using var redirector = new ProcessOutRedirector(process);
            
        redirector.StopRedirection(); // Cancel before starting

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => redirector.StartRedirectionAsync());
    }

    #endregion

    #region RedirectAndWait Tests

    [Fact]
    public void RedirectAndWait_WithQuickProcess_ReturnsExitCode()
    {
        // Arrange
        using var process = CreateTestProcess("Quick test");
        using var redirector = new ProcessOutRedirector(process);
            
        process.Start();

        // Act
        redirector.RedirectAndWait(5000);

        // Assert
        Assert.Equal(0, process.ExitCode); 
        Assert.True(process.HasExited);
    }

    [Fact]
    public void RedirectAndWait_WithTimeout_ReturnsWhenProcessCompletes()
    {
        // Arrange
        using var process = CreateTestProcess("Timeout test");
        using var redirector = new ProcessOutRedirector(process);
            
        process.Start();
        var stopwatch = Stopwatch.StartNew();

        // Act
        redirector.RedirectAndWait(5000);

        // Assert
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds < 5000); // Should complete quickly
        Assert.Equal(0, process.ExitCode);
    }

    #endregion

    #region RedirectAndWaitAsync Tests

    [Fact]
    public async Task RedirectAndWaitAsync_WithQuickProcess_ReturnsExitCode()
    {
        // Arrange
        using var process = CreateTestProcess("Async test");
        using var redirector = new ProcessOutRedirector(process);
            
        process.Start();

        // Act
        await redirector.RedirectAndWaitAsync(5000);

        // Assert
        Assert.Equal(0, process.ExitCode); 
        Assert.True(process.HasExited);
    }

    [Fact]
    public async Task RedirectAndWaitAsync_WithInfiniteTimeout_CompletesWhenProcessExits()
    {
        // Arrange
        using var process = CreateTestProcess("Infinite timeout test");
        using var redirector = new ProcessOutRedirector(process);
            
        process.Start();

        // Act
        await redirector.RedirectAndWaitAsync(-1);

        // Assert
        Assert.Equal(0, process.ExitCode); 
        Assert.True(process.HasExited);
    }

    [Fact]
    public async Task RedirectAndWaitAsync_WithLongRunningProcess_HandlesTimeout()
    {
        // Arrange
        using var process = CreateLongRunningProcess();
        using var redirector = new ProcessOutRedirector(process);
            
        process.Start();
        var stopwatch = Stopwatch.StartNew();

        // Act
        await Assert.ThrowsAsync<TaskCanceledException>(async () => await redirector.RedirectAndWaitAsync(1000)); // Short timeout

        // Assert
        stopwatch.Stop();
        
        // The process should be killed due to timeout
        Assert.True(stopwatch.ElapsedMilliseconds >= 1000);
        Assert.False(process.HasExited);
        
        process.Kill();
    }

    #endregion
    
}