namespace Servus.Core;

public abstract class AwaitableCondition
{
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new ();

    protected AwaitableCondition(int timeoutMilliseconds)
        : this(new CancellationTokenSource(timeoutMilliseconds).Token)
    {
    }

    protected AwaitableCondition(CancellationToken token, bool throwExceptionIfCanceled = true)
    {
        token.Register(() =>
        {
            OnCanceled();
            if (throwExceptionIfCanceled)
            {
                _taskCompletionSource.TrySetCanceled();
            }
            else
            {
                _taskCompletionSource.TrySetResult(false);
            }
        });
    }

    public Task<bool> WaitAsync()
    {
        OnConditionChanged();
        return _taskCompletionSource.Task;
    }

    protected virtual bool OnConditionChanged()
    {
        if (Evaluate())
        {
            OnSuccess();
            _taskCompletionSource.TrySetResult(true);
            return true;
        }

        OnFailed();
        return false;
    }

    protected abstract bool Evaluate();

    protected virtual void OnSuccess()
    {
    }

    protected virtual void OnFailed()
    {
    }

    protected virtual void OnCanceled()
    {
    }
}