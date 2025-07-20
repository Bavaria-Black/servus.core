using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Servus.Core.Tests;

[TestClass]
public class AwaitableConditionTests
{
    [TestMethod]
    public async Task AwaitableCondition_can_be_fulfilled()
    {
        var condition = new MockAwaitableCondition(1000);
        var waitTask = Task.Run(condition.WaitAsync);

        condition.Count += 2;
        var success = await waitTask;
            
        Assert.IsTrue(success);
    }

    [TestMethod]
    public async Task AwaitableCondition_timeouts()
    {
        var condition = new MockAwaitableCondition(10);
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
        {
            await condition.WaitAsync();
        });
    }

    [TestMethod]
    public async Task AwaitableCondition_can_be_canceled()
    {
        var cts = new CancellationTokenSource();
        var condition = new MockAwaitableCondition(cts.Token);
        cts.Cancel();
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
        {
            await condition.WaitAsync();
        });
    }

    [TestMethod]
    public async Task AwaitableCondition_returns_false_when_cancelled_and_told_to()
    {
        var cts = new CancellationTokenSource();
        var condition = new MockAwaitableCondition(cts.Token, false);
        cts.Cancel();
        var returnValue = await condition.WaitAsync();
        Assert.IsFalse(returnValue);
    }
}

internal class MockAwaitableCondition : AwaitableCondition
{
    private int _count = 0;
    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            OnConditionChanged();
        }
    }
        
    public MockAwaitableCondition(int timeoutMilliseconds) 
        : base(timeoutMilliseconds)
    {
    }

    public MockAwaitableCondition(CancellationToken token, bool throwExceptionIfCanceled = true) 
        : base(token, throwExceptionIfCanceled)
    {
    }

    protected override bool Evaluate()
    {
        return Count > 1;
    }
}