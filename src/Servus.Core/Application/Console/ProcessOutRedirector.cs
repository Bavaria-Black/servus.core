using System.Diagnostics;
using System.IO;

/// <summary>
/// Redirects a process's StandardOutput and StandardError to the current console output.
/// </summary>
public class ProcessOutRedirector : IDisposable
{
    private readonly Process _process;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task _stdOutTask;
    private Task _stdErrTask;
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

        if (process.StartInfo.FileName == null)
            throw new InvalidOperationException("Process has already started or is not properly configured");

        // Configure the process for redirection
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

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
    public int RedirectAndWait(int timeoutMs = -1)
    {
        return RedirectAndWaitAsync(timeoutMs).GetAwaiter().GetResult();
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

        if (_process.HasExited)
            throw new InvalidOperationException("Process has already exited");

        if (_stdOutTask != null || _stdErrTask != null)
            throw new InvalidOperationException("Redirection is already active");

        // Start the output redirection tasks
        _stdOutTask = RedirectStreamAsync(
            _process.StandardOutput,
            Console.Out,
            _cancellationTokenSource.Token);

        _stdErrTask = RedirectStreamAsync(
            _process.StandardError,
            Console.Error,
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
    }

    /// <summary>
    /// Starts redirection and waits for the process to exit.
    /// </summary>
    /// <param name="timeoutMs">Optional timeout in milliseconds. -1 for infinite timeout.</param>
    /// <returns>The process exit code</returns>
    public async Task<int> RedirectAndWaitAsync(int timeoutMs = -1)
    {
        var redirectionTask = StartRedirectionAsync();
        
        Task processExitTask;
        if (timeoutMs > 0)
        {
            using var timeoutCts = new CancellationTokenSource(timeoutMs);
            processExitTask = WaitForExitAsync(_process, timeoutCts.Token);
        }
        else
        {
            processExitTask = WaitForExitAsync(_process, CancellationToken.None);
        }

        // Wait for either the process to exit or redirection to complete
        await Task.WhenAny(redirectionTask, processExitTask);

        // Ensure we wait for the process to actually exit
        await processExitTask;

        // Give a moment for any remaining output to be processed
        await Task.Delay(100);

        return _process.ExitCode;
    }

    /// <summary>
    /// Creates a ProcessOutRedirector with a pre-configured process.
    /// </summary>
    /// <param name="fileName">The executable file to run</param>
    /// <param name="arguments">Command line arguments</param>
    /// <param name="workingDirectory">Working directory for the process</param>
    /// <returns>A configured ProcessOutRedirector</returns>
    public static ProcessOutRedirector Create(string fileName, string arguments = "", string workingDirectory = null)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
            }
        };

        return new ProcessOutRedirector(process);
    }

    private static async Task RedirectStreamAsync(
        StreamReader sourceStream,
        TextWriter targetStream,
        CancellationToken cancellationToken)
    {
        try
        {
            string line;
            while ((line = await sourceStream.ReadLineAsync()) != null)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                // Write to target stream (Console.Out or Console.Error)
                await targetStream.WriteLineAsync(line);
                await targetStream.FlushAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
    }

    private static async Task WaitForExitAsync(Process process, CancellationToken cancellationToken)
    {
        while (!process.HasExited && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(50, cancellationToken);
        }

        if (!process.HasExited)
        {
            try
            {
                process.Kill();
                await Task.Delay(1000, CancellationToken.None); // Give time for cleanup
            }
            catch
            {
                // Ignore errors when killing process
            }
        }
    }

    /// <summary>
    /// Gets the underlying process.
    /// </summary>
    public Process Process => _process;

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
            Task.WaitAll(new[] { _stdOutTask, _stdErrTask }.Where(t => t != null).ToArray(), 1000);
        }
        catch
        {
            // Ignore disposal errors
        }

        _disposed = true;
    }
}