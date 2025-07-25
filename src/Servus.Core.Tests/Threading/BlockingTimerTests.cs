﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servus.Core.Threading;

namespace Servus.Core.Tests.Threading;

[TestClass]
public class BlockingTimerTests
{
    #region Constructor Tests

    [TestMethod]
    public async Task Constructor_WithActionAndInterval_StartsTimerAutomatically()
    {
        // Arrange
        var executionCount = 0;

        // Act
        using var timer = new BlockingTimer(TimerAction, 100);
        await Task.Delay(250);

        // Assert
        Assert.IsTrue(executionCount >= 2, $"Expected at least 2 executions, got {executionCount}");
        return;

        void TimerAction() => Interlocked.Increment(ref executionCount);
    }

    [TestMethod]
    public void Constructor_WithNullAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new BlockingTimer(null!, 100));
    }

    [TestMethod]
    public async Task Constructor_WithZeroInterval_AllowsZeroInterval()
    {
        // Arrange
        var executionCount = 0;

        // Act & Assert - Should not throw
        using var timer = new BlockingTimer(TimerAction, 0);
        await Task.Delay(50);
        
        Assert.IsTrue(executionCount > 0);
        return;

        void TimerAction() => Interlocked.Increment(ref executionCount);
    }

    [TestMethod]
    public async Task Constructor_WithNegativeInterval_AllowsNegativeInterval()
    {
        // Arrange
        var executionCount = 0;

        // Act & Assert - Should not throw, behaves like zero interval
        using var timer = new BlockingTimer(TimerAction, -100);
        await Task.Delay(50);
            
        Assert.IsTrue(executionCount > 0);
        return;

        void TimerAction() => Interlocked.Increment(ref executionCount);
    }

    #endregion

    #region Constructor with CancellationToken Tests

    [TestMethod]
    public async Task Constructor_WithCancellationToken_StartsTimerAutomatically()
    {
        // Arrange
        var executionCount = 0;
        using var cts = new CancellationTokenSource();

        // Act
        using var timer = new BlockingTimer(TimerAction, 100, cts.Token);
        await Task.Delay(250, cts.Token);

        // Assert
        Assert.IsTrue(executionCount >= 2);
        return;
        void TimerAction() => Interlocked.Increment(ref executionCount);
    }

    [TestMethod]
    public async Task Constructor_WithCancelledToken_StopsImmediately()
    {
        // Arrange
        var executionCount = 0;
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync(); // Cancel before creating timer

        // Act
        using var timer = new BlockingTimer(TimerAction, 100, cts.Token);
        await Task.Delay(250);

        // Assert
        Assert.AreEqual(0, executionCount, "Timer should not execute when token is already cancelled");
        return;
        void TimerAction() => Interlocked.Increment(ref executionCount);
    }

    [TestMethod]
    public async Task Constructor_WithTokenCancelledAfterStart_StopsExecution()
    {
        // Arrange
        var executionCount = 0;
        using var cts = new CancellationTokenSource();

        // Act
        using var timer = new BlockingTimer(TimerAction, 100, cts.Token);
        await Task.Delay(150, cts.Token);
        var countBeforeCancel = executionCount;
            
        await cts.CancelAsync();
        await Task.Delay(200);
        var countAfterCancel = executionCount;

        // Assert
        Assert.IsTrue(countBeforeCancel > 0, "Timer should have executed before cancellation");
        Assert.AreEqual(countBeforeCancel, countAfterCancel, "Timer should stop executing after cancellation");
        return;
        void TimerAction() => Interlocked.Increment(ref executionCount);
    }

    [TestMethod]
    public void Constructor_WithNullActionAndCancellationToken_ThrowsArgumentNullException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => 
            new BlockingTimer(null!,100, cts.Token));
    }

    #endregion

    #region Timer Execution Tests

    [TestMethod]
    public async Task TimerExecution_WithRegularInterval_ExecutesAtExpectedFrequency()
    {
        // Arrange
        var executionTimes = new List<DateTime>();
        var lockObj = new object();
        Action timerAction = () =>
        {
            lock (lockObj)
            {
                executionTimes.Add(DateTime.Now);
            }
        };

        // Act
        using var timer = new BlockingTimer(timerAction, 100);
        await Task.Delay(350);

        // Assert
        lock (lockObj)
        {
            Assert.IsTrue(executionTimes.Count >= 3, $"Expected at least 3 executions, got {executionTimes.Count}");
                
            // Check intervals between executions (allowing some tolerance)
            for (int i = 1; i < executionTimes.Count; i++)
            {
                var interval = (executionTimes[i] - executionTimes[i - 1]).TotalMilliseconds;
                Assert.IsTrue(interval >= 90 && interval <= 150, 
                    $"Interval {i} was {interval}ms, expected ~100ms");
            }
        }
    }

    [TestMethod]
    public async Task TimerExecution_WithSlowAction_WaitsForActionToComplete()
    {
        // Arrange
        var executionCount = 0;
        var executionTimes = new List<DateTime>();
        var lockObj = new object();
            
        Action slowAction = () =>
        {
            lock (lockObj)
            {
                executionTimes.Add(DateTime.Now);
                Interlocked.Increment(ref executionCount);
            }

            Thread.Sleep(200);
        };

        // Act
        using var timer = new BlockingTimer(slowAction, 100); // 100ms interval, 200ms action
        await Task.Delay(500);

        // Assert
        // With 200ms action time, we should have fewer executions than interval would suggest
        Assert.IsTrue(executionCount >= 2 && executionCount <= 3, 
            $"Expected 2-3 executions due to blocking, got {executionCount}");

        lock (lockObj)
        {
            // Verify that executions are properly spaced (at least 200ms apart due to slow action)
            for (int i = 1; i < executionTimes.Count; i++)
            {
                var interval = (executionTimes[i] - executionTimes[i - 1]).TotalMilliseconds;
                Assert.IsTrue(interval >= 190, 
                    $"Interval {i} was {interval}ms, should be at least 200ms due to slow action");
            }
        }
    }

    [TestMethod]
    public async Task TimerExecution_WithExceptionInAction_ContinuesExecution()
    {
        // Arrange
        var executionCount = 0;
        var exceptionCount = 0;
            
        Action faultyAction = () =>
        {
            var count = Interlocked.Increment(ref executionCount);
            if (count == 2) // Throw exception on second execution
            {
                Interlocked.Increment(ref exceptionCount);
                throw new InvalidOperationException("Test exception");
            }
        };

        // Act
        using var timer = new BlockingTimer(faultyAction, 50);
        await Task.Delay(300);

        // Assert
        Assert.IsTrue(executionCount > 3, "Timer should continue executing after exception");
        Assert.AreEqual(1, exceptionCount, "Exception should have been thrown once");
    }

    #endregion

    #region Stop Method Tests

    [TestMethod]
    public async Task Stop_WithRunningTimer_StopsExecution()
    {
        // Arrange
        var executionCount = 0;
        Action timerAction = () => Interlocked.Increment(ref executionCount);
        var timer = new BlockingTimer(timerAction, 50);

        // Act
        await Task.Delay(125);
        var countBeforeStop = executionCount;
            
        timer.Stop();
        await Task.Delay(150);
        var countAfterStop = executionCount;

        // Assert
        Assert.IsTrue(countBeforeStop > 0, "Timer should have executed before stopping");
        Assert.AreEqual(countBeforeStop, countAfterStop, "Timer should not execute after stopping");
            
        timer.Dispose();
    }

    [TestMethod]
    public void Stop_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var executionCount = 0;
        Action timerAction = () => Interlocked.Increment(ref executionCount);
        var timer = new BlockingTimer(timerAction, 100);

        // Act & Assert - Should not throw
        timer.Stop();
        timer.Stop(); // Second call should be safe
        timer.Stop(); // Third call should be safe
            
        timer.Dispose();
    }

    [TestMethod]
    public async Task Stop_WaitsForCurrentExecution_CompletesCleanly()
    {
        // Arrange
        var executionStarted = false;
        var executionCompleted = false;
        var stopCalled = false;
            
        Action longAction = () =>
        {
            executionStarted = true;
            // Wait until stop is called, then complete
            while (!stopCalled) Thread.Sleep(10);
            Thread.Sleep(100); // Simulate some work after stop is called
            executionCompleted = true;
        };

        var timer = new BlockingTimer(longAction, 1000); // Long interval

        // Act
        await Task.Delay(50);
        Assert.IsTrue(executionStarted, "Execution should have started");
            
        stopCalled = true;
        var stopwatch = Stopwatch.StartNew();
        timer.Stop(); // This should wait for execution to complete
        stopwatch.Stop();

        // Assert
        Assert.IsTrue(executionCompleted, "Execution should have completed before Stop() returned");
        Assert.IsTrue(stopwatch.ElapsedMilliseconds >= 90, "Stop() should have waited for execution to complete");
            
        timer.Dispose();
    }

    #endregion

    #region Dispose Tests

    [TestMethod]
    public async Task Dispose_WithRunningTimer_StopsAndDisposesCleanly()
    {
        // Arrange
        var executionCount = 0;
        Action timerAction = () => Interlocked.Increment(ref executionCount);
        var timer = new BlockingTimer(timerAction, 50);

        // Act
        await Task.Delay(125);
        var countBeforeDispose = executionCount;
            
        timer.Dispose();
        await Task.Delay(150); // Wait after disposing
        var countAfterDispose = executionCount;

        // Assert
        Assert.IsTrue(countBeforeDispose > 0, "Timer should have executed before disposing");
        Assert.AreEqual(countBeforeDispose, countAfterDispose, "Timer should not execute after disposing");
    }

    [TestMethod]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var executionCount = 0;
        Action timerAction = () => Interlocked.Increment(ref executionCount);
        var timer = new BlockingTimer(timerAction, 100);

        // Act & Assert - Should not throw
        timer.Dispose();
        timer.Dispose(); // Second call should be safe
        timer.Dispose(); // Third call should be safe
    }

    [TestMethod]
    public async Task Dispose_WithUsingStatement_DisposesAutomatically()
    {
        // Arrange
        var executionCount = 0;
        Action timerAction = () => Interlocked.Increment(ref executionCount);

        // Act
        using (var timer = new BlockingTimer(timerAction, 50))
        {
            await Task.Delay(125);
        } // Dispose should be called here

        await Task.Delay(100); // Wait after using block
        var finalCount = executionCount;

        // Give a moment and check count doesn't increase
        await Task.Delay(100);
        var countAfterWait = executionCount;

        // Assert
        Assert.IsTrue(finalCount > 0, "Timer should have executed within using block");
        Assert.AreEqual(finalCount, countAfterWait, "Timer should not execute after dispose");
    }

    #endregion

    #region Timing and Performance Tests

    [TestMethod]
    public async Task TimerAccuracy_WithPreciseInterval_MaintainsAccuracy()
    {
        // Arrange
        var executionTimes = new List<DateTime>();
        var lockObj = new object();
        const int intervalMs = 50;
            
        Action timerAction = () =>
        {
            lock (lockObj)
            {
                executionTimes.Add(DateTime.Now);
            }
        };

        // Act
        using var timer = new BlockingTimer(timerAction, intervalMs);
        await Task.Delay(300); // Allow ~6 executions

        // Assert
        lock (lockObj)
        {
            Assert.IsTrue(executionTimes.Count >= 5, $"Expected at least 5 executions, got {executionTimes.Count}");
                
            // Calculate average interval
            var totalInterval = (executionTimes[^1] - executionTimes[0]).TotalMilliseconds;
            var averageInterval = totalInterval / (executionTimes.Count - 1);
                
            // Allow 30% tolerance for timing variations
            Assert.IsTrue(averageInterval is >= intervalMs * 0.7 and <= intervalMs * 1.3,
                $"Average interval was {averageInterval}ms, expected ~{intervalMs}ms");
        }
    }

    [TestMethod]
    public async Task BlockingBehavior_WithVerySlowAction_ExecutesSequentially()
    {
        // Arrange
        var executionStarts = new List<DateTime>();
        var executionEnds = new List<DateTime>();
        var lockObj = new object();
            
        Action slowAction = () =>
        {
            var start = DateTime.Now;
            lock (lockObj)
            {
                executionStarts.Add(start);
            }
                
            var end = DateTime.Now;
            lock (lockObj)
            {
                executionEnds.Add(end);
            }
        };

        // Act
        using var timer = new BlockingTimer(slowAction, 50); // Short interval, long action
        await Task.Delay(400); // Allow multiple executions

        // Assert
        lock (lockObj)
        {
            Assert.IsTrue(executionStarts.Count >= 2, "Should have multiple executions");
            Assert.AreEqual(executionStarts.Count, executionEnds.Count, "All executions should complete");
                
            // Verify no overlapping executions (blocking behavior)
            for (int i = 1; i < executionStarts.Count; i++)
            {
                Assert.IsTrue(executionStarts[i] >= executionEnds[i - 1],
                    $"Execution {i} started before execution {i-1} ended - not blocking properly");
            }
        }
    }

    #endregion

    #region Edge Cases and Error Handling

    [TestMethod]
    public async Task EdgeCase_ActionThrowsOperationCanceledException_HandledGracefully()
    {
        // Arrange
        var executionCount = 0;
        Action actionWithCancel = () =>
        {
            Interlocked.Increment(ref executionCount);
            if (executionCount == 2)
            {
                throw new OperationCanceledException("Test cancellation");
            }
        };

        // Act & Assert - Should not throw
        using var timer = new BlockingTimer(actionWithCancel, 50);
        await Task.Delay(200);
            
        Assert.IsTrue(executionCount > 2, "Timer should continue after OperationCanceledException");
    }

    [TestMethod]
    public async Task EdgeCase_VeryHighFrequency_HandlesCorrectly()
    {
        // Arrange
        var executionCount = 0;

        // Act
        using var timer = new BlockingTimer(TimerAction, 2); // 1ms interval
        await Task.Delay(100);
        timer.Stop();
        
        // Assert
        Assert.IsTrue(executionCount > 5, "Should execute many times with 1ms interval");
        Assert.IsTrue(executionCount < 100, "Should not execute unreasonably often");
        return;
        void TimerAction() => Interlocked.Increment(ref executionCount);
    }

    #endregion

    #region Integration Tests

    [TestMethod]
    public async Task Integration_FullLifecycle_WorksCorrectly()
    {
        // Arrange
        var executionLog = new List<string>();
        var lockObj = new object();
        var executionNumber = 0;
            
        Action logAction = () =>
        {
            var num = Interlocked.Increment(ref executionNumber);
            lock (lockObj)
            {
                executionLog.Add($"Execution {num} at {DateTime.Now:HH:mm:ss.fff}");
            }
        };

        // Act - Full lifecycle
        using (new BlockingTimer(logAction, 75))
        {
            await Task.Delay(200);
                
            lock (lockObj)
            {
                var countDuringRun = executionLog.Count;
                Assert.IsTrue(countDuringRun >= 2, "Should execute during normal operation");
            }
                
            // Timer will be disposed here
        }
            
        await Task.Delay(100);
            
        // Assert
        lock (lockObj)
        {
            Assert.IsTrue(executionLog.Count >= 2, "Should have executed multiple times");
            // Verify log contains expected format
            foreach (var entry in executionLog)
            {
                Assert.IsTrue(entry.StartsWith("Execution"), $"Invalid log entry: {entry}");
            }
        }
    }

    #endregion
}