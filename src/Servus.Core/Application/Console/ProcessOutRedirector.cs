using System.Diagnostics;
using System.IO;

namespace Servus.Core.Application.Console;

/// <summary>
/// Redirects a process's StandardOutput and StandardError to the current console output.
/// </summary>
public class ProcessOutRedirector : IDisposable
{
    private readonly Process _process;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task? _stdOutTask;
    private Task? _stdErrTask;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the ProcessOutRedirector class.
    /// </summary>
    /// <param name="process">The process to redirect output from</param>
    /// <exception cref="ArgumentNullException">Thrown when process is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when process output is not redirected</exception>
    public ProcessOutRedirector(Process process)
    {
        ArgumentNullException.ThrowIfNull(process);

        _process = process;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Starts redirecting the process output to the console (blocking version).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when redirection is already active</exception>
    public void StartRedirection()
    {
        StartRedirectionAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Starts redirection and waits for the process to exit (blocking version).
    /// </summary>
    /// <param name="timeoutMs">Optional timeout in milliseconds. -1 for infinite timeout.</param>
    /// <returns>The process exit code</returns>
    public void RedirectAndWait(int timeoutMs = -1)
    {
        RedirectAndWaitAsync(timeoutMs).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Starts redirecting the process output to the console.
    /// </summary>
    /// <returns>A task representing the redirection operation</returns>
    /// <exception cref="InvalidOperationException">Thrown when redirection is already active</exception>
    public async Task StartRedirectionAsync()
    {
        if (_cancellationTokenSource.IsCancellationRequested)
            throw new InvalidOperationException("Redirection was already cancelled");

        if (_stdOutTask != null || _stdErrTask != null)
            throw new InvalidOperationException("Redirection is already active");

        var hasExited = false;
        try
        {
            if (_process.HasExited)
            {
                hasExited = true;
                throw new InvalidOperationException("Process has already exited");
            }
        }
        catch (Exception) when (!hasExited) // Accessing HasExited when process not started also throws an exception
        {
            // start process if not started
            _process.Start();
        }

        // Configure the process for redirection
        _process.StartInfo.UseShellExecute = false;
        _process.StartInfo.RedirectStandardOutput = true;
        _process.StartInfo.RedirectStandardError = true;
        
        // Start the output redirection tasks
        _stdOutTask = RedirectStreamAsync(
            _process.StandardOutput,
            System.Console.Out,
            _cancellationTokenSource.Token);

        _stdErrTask = RedirectStreamAsync(
            _process.StandardError,
            System.Console.Error,
            _cancellationTokenSource.Token);

        // Wait for both tasks to complete (when process exits)
        await Task.WhenAll(_stdOutTask, _stdErrTask);
    }

    /// <summary>
    /// Stops the redirection of process output.
    /// </summary>
    public void StopRedirection()
    {
        _cancellationTokenSource.Cancel();
        _process.StartInfo.RedirectStandardError = false;
        _process.StartInfo.RedirectStandardOutput = false;
    }

    /// <summary>
    /// Starts redirection and waits for the process to exit.
    /// </summary>
    /// <param name="timeoutMs">Optional timeout in milliseconds. -1 for infinite timeout.</param>
    /// <returns>The process exit code</returns>
    public async Task RedirectAndWaitAsync(int timeoutMs = -1)
    {
        var redirectionTask = StartRedirectionAsync();
        using var cts = timeoutMs < 0 ?  new CancellationTokenSource() : new CancellationTokenSource(timeoutMs);
        var processExitTask = _process.WaitForExitAsync(cts.Token);
        
        // Wait for either the process to exit or redirection to complete
        await Task.WhenAny(redirectionTask, processExitTask);

        // Ensure we wait for the process to actually exit
        await processExitTask;
    }

    private static async Task RedirectStreamAsync(
        StreamReader sourceStream,
        TextWriter targetStream,
        CancellationToken cancellationToken)
    {
        try
        {
            while (await sourceStream.ReadLineAsync(cancellationToken) is { } line)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                // Write to target stream (Console.Out or Console.Error)
                await targetStream.WriteLineAsync(line);
                await targetStream.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
    }
    
    /// <summary>
    /// Disposes the ProcessOutRedirector and stops any active redirection.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        StopRedirection();

        try
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            // Wait a bit for tasks to complete
            Task.WaitAll(new[] { _stdOutTask, _stdErrTask }.Where(t => t != null).ToArray()!, 1000);
        }
        catch
        {
            // Ignore disposal errors
        }

        _disposed = true;
    }
}