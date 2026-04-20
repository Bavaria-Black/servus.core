# AwaitableCondition

`AwaitableCondition` is an abstract base for "wait until X is true" with first-class timeout and cancellation support. You subclass it, implement `Evaluate()`, wire up whatever events tell you the world has changed, and consumers just `await WaitAsync()`.

## Key features

- **Asynchronous waiting** — `async/await` end-to-end, no polling unless you add it yourself
- **Timeout support** — configurable; choose whether a timeout throws or returns `false`
- **Cancellation** — cooperative via `CancellationToken`
- **Extensible hooks** — `OnSuccess` / `OnFailed` / `OnCanceled` for cleanup
- **No spin** — `Evaluate` is only called when you call `OnConditionChanged()`

## Minimal shape

```csharp
public Task<bool> WaitAsync();
```

- Returns `true` when `Evaluate()` returned `true`.
- Returns `false` when the evaluation ran and returned `false` before timeout (or when the timeout/cancel fired and `throwExceptionIfCanceled` is `false`).
- Throws `OperationCanceledException` on timeout/cancel when `throwExceptionIfCanceled` is `true`.

```csharp
var condition = new DatabaseConnectionCondition("Server=localhost;…", timeoutMilliseconds: 30_000);

try
{
    if (await condition.WaitAsync())
    {
        Console.WriteLine("Database is ready!");
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Timeout waiting for database connection");
}
```

## Implementation rules

```csharp
protected abstract bool Evaluate();
```

- **Must** be implemented by derived classes.
- **Should** be fast and non-blocking — it runs on whatever thread signalled `OnConditionChanged`.
- **Should not** throw; return `false` on failure.

You call `OnConditionChanged()` from whatever event, poll, or observer signals that the world has changed. The base class then calls `Evaluate` to decide whether the wait is done.

## State hooks

Override to plug into lifecycle events (great for cleanup):

- `OnSuccess` — condition was met
- `OnFailed` — evaluation returned `false`
- `OnCanceled` — timeout or cancellation fired

```csharp
protected override void OnCanceled()
    => logger.LogWarning("Condition wait was canceled after timeout");
```

## Example: file appears

```csharp
public class FileCreatedCondition : AwaitableCondition
{
    private readonly string _filePath;
    private readonly FileSystemWatcher _watcher;

    public FileCreatedCondition(string filePath, CancellationToken cancellationToken)
        : base(cancellationToken, throwExceptionIfCanceled: false)
    {
        _filePath = filePath;

        var directory = Path.GetDirectoryName(filePath)!;
        var fileName  = Path.GetFileName(filePath);

        _watcher = new FileSystemWatcher(directory, fileName);
        _watcher.Created += (_, _) => OnConditionChanged();
        _watcher.EnableRaisingEvents = true;
    }

    protected override bool Evaluate()        => File.Exists(_filePath);
    protected override void OnSuccess()       => _watcher.Dispose();
    protected override void OnCanceled()      => _watcher.Dispose();
}

using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
var condition = new FileCreatedCondition(@"C:\temp\ready.flag", cts.Token);
bool ok = await condition.WaitAsync();
```

## Example: HTTP service ready

```csharp
public class ServiceReadinessCondition : AwaitableCondition
{
    private readonly HttpClient _http;
    private readonly string _healthUrl;
    private readonly Timer _pollTimer;

    public ServiceReadinessCondition(string healthUrl, TimeSpan pollInterval, int timeoutMs)
        : base(timeoutMs)
    {
        _healthUrl = healthUrl;
        _http = new HttpClient();
        _pollTimer = new Timer(_ => OnConditionChanged(), null, TimeSpan.Zero, pollInterval);
    }

    protected override bool Evaluate()
    {
        try   { return _http.GetAsync(_healthUrl).Result.IsSuccessStatusCode; }
        catch { return false; }
    }

    protected override void OnSuccess()  { _pollTimer.Dispose(); _http.Dispose(); }
    protected override void OnCanceled() { _pollTimer.Dispose(); _http.Dispose(); }
}
```

## API

```csharp
public abstract class AwaitableCondition
{
    protected AwaitableCondition(int timeoutMilliseconds);
    protected AwaitableCondition(CancellationToken token, bool throwExceptionIfCanceled = true);

    public Task<bool> WaitAsync();

    protected abstract bool Evaluate();
    protected virtual void OnSuccess();
    protected virtual void OnFailed();
    protected virtual void OnCanceled();
}
```

## Common use cases

- Service startup — wait for databases, APIs, external services
- File operations — wait for files to be created or become accessible
- State synchronisation — wait for application state changes
- Testing — predictable delays and conditions in integration tests
