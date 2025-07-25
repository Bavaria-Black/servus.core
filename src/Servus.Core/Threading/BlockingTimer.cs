namespace Servus.Core.Threading;

/// <summary>
/// Timer that ensures, that the given timer action is only started once at a time.
/// If the action run's longer then the specified interval, it waits until the action is executed.
/// The timer start's once it is constructed and can be stopped via a cancellation token.
/// </summary>
public sealed class BlockingTimer : IDisposable
{
    private readonly Action _timerAction;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly double _intervalMilliseconds;
    private Task? _task;

    /// <summary>
    /// Creates and starts the timer, until the cancellation via the provided cancellation token is requested
    /// </summary>
    public BlockingTimer(Action timerAction, double intervalInMilliseconds, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(timerAction);
        ArgumentNullException.ThrowIfNull(intervalInMilliseconds);
        
        _timerAction = timerAction;
        _intervalMilliseconds = intervalInMilliseconds;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Start();
    }

    private void Start()
    {
        try
        {
            if (_cancellationTokenSource != null)
                _task = Task.Factory.StartNew(ExecuteTimerLoop, _cancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }
        catch (OperationCanceledException)
        {
            // No error handling required.
        }
    }

    /// <summary>
    /// Stops the timer and wait's until it's stopped
    /// </summary>
    public void Stop()
    {
        _cancellationTokenSource?.Cancel();

        // Wait for the scheduled task to complete
        try
        {

            _task?.Wait();
        }
        catch (Exception)
        {
            // nop
        }
    }

    private async Task ExecuteTimerLoop()
    {
        while ((!_cancellationTokenSource?.Token.IsCancellationRequested) ?? false)
        {
            try
            {
                var nextExecutionTime = DateTime.Now.AddMilliseconds(_intervalMilliseconds);
                _timerAction();
                await DelayUntilNextExecutionTime(nextExecutionTime).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // No error handling required
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    private Task DelayUntilNextExecutionTime(DateTime nextExecutionTime)
    {
        var waitFor = nextExecutionTime - DateTime.Now;

        return waitFor > TimeSpan.Zero
            ? Task.Delay(waitFor, _cancellationTokenSource?.Token ?? CancellationToken.None)
            : Task.CompletedTask;
    }

    public void Dispose()
    {
        Stop();
        
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;

        _task?.Dispose();
        _task = null;
    }
}