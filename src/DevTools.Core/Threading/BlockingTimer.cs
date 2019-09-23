using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTools.Core.Threading
{
    /// <summary>
    /// Timer that ensures, that the given timer action is only started once at a time.
    /// If the action run's longer then the specified interval, it waits until the action is executed.
    /// The timer start's once it is constructed and can be stopped via a cancellation token.
    /// </summary>
    public class BlockingTimer
    {
        private readonly Action _timerAction;
        private readonly CancellationToken _cancellationToken;
        private readonly double _intervalMilliseconds;

        /// <summary>
        /// Creates and starts the timer, until the cancellation via the provided cancellation token is requested
        /// </summary>
        public BlockingTimer(Action timerAction, CancellationToken cancellationToken, double intervalInMilliseconds)
        {
            _timerAction = timerAction;
            _cancellationToken = cancellationToken;
            _intervalMilliseconds = intervalInMilliseconds;
            Start();
        }

        private void Start()
        {
            try
            {
                Task.Run(ExecuteTimerLoop, _cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // No error handling required.
            }
        }

        private async Task ExecuteTimerLoop()
        {
            do
            {
                try
                {
                    var nextExecutionTime = DateTime.Now.AddMilliseconds(_intervalMilliseconds);
                    _timerAction();
                    await DelayUntilNextExecutionTime(nextExecutionTime);
                }
                catch (OperationCanceledException)
                {
                    // No error handling required
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            } while (!_cancellationToken.IsCancellationRequested);
        }

        private async Task DelayUntilNextExecutionTime(DateTime nextExecutionTime)
        {
            var waitFor = nextExecutionTime - DateTime.Now;

            if (waitFor > TimeSpan.Zero)
            {
                await Task.Delay(waitFor, _cancellationToken);
            }
        }
    }
}
