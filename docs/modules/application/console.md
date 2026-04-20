# Console Utilities

`ServusConsole` is a small set of helpers for colored output and structured key/value printing, and `ProcessOutRedirector` pipes the stdout/stderr of a spawned process into your own console.

## `ServusConsole` — colored output

```csharp
using Servus.Core.Application.Console;

ServusConsole.WriteColored("OK ", ConsoleColor.Green);
ServusConsole.WriteLineColored("All systems go.", ConsoleColor.Gray);

ServusConsole.PrintLine();                 // ===== (80 chars)
ServusConsole.PrintLine(40, '─');          // 40 U+2500 chars
```

## `ServusConsole` — key/value printing

Useful for startup diagnostics ("what's my configuration right now?"). Keys are colored and padded to a fixed width; values are left plain.

```csharp
ServusConsole.PrintKeyValue("Environment", "Production");
ServusConsole.PrintKeyValue("Region",      "eu-central-1");
ServusConsole.PrintKeyValue("Build",       "2026.04.20+abc");
```

Output:

```
  Environment   : Production
  Region        : eu-central-1
  Build         : 2026.04.20+abc
```

The full overload set takes alignment width, key color, and indent:

```csharp
public static class ServusConsole
{
    public static void WriteColored(string value, ConsoleColor color);
    public static void WriteLineColored(string value, ConsoleColor color);

    public static void PrintLine(int length = 80, char lineChar = '=');

    public static void PrintKeyValue<TKey, TValue>(
        KeyValuePair<TKey, TValue> keyValuePair,
        int width = 14,
        ConsoleColor keyColor = ConsoleColor.DarkCyan,
        int indent = 2);

    public static void PrintKeyValue(
        KeyValuePair<string, string> keyValuePair,
        int width = 14,
        ConsoleColor keyColor = ConsoleColor.DarkCyan,
        int indent = 2);

    public static void PrintKeyValue(
        string key,
        string value,
        int width = 14,
        ConsoleColor keyColor = ConsoleColor.DarkCyan,
        int indent = 2);
}
```

## `ProcessOutRedirector` — stream child-process output

When you spawn a child `Process` (e.g. a CLI tool) you usually want its output on *your* console in real time. `ProcessOutRedirector` does exactly that:

```csharp
using var process = new Process
{
    StartInfo =
    {
        FileName = "git",
        Arguments = "status",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    }
};
process.Start();

using var redirector = new ProcessOutRedirector(process);
await redirector.RedirectAndWaitAsync();   // streams stdout/stderr, awaits exit
```

### Fire-and-forget redirection

If you need finer control over the lifecycle, `StartRedirectionAsync()` and `StopRedirection()` are separate:

```csharp
using var redirector = new ProcessOutRedirector(process);
_ = redirector.StartRedirectionAsync();

// … do other work …

redirector.StopRedirection();
```

### Full API

```csharp
public sealed class ProcessOutRedirector : IDisposable
{
    public ProcessOutRedirector(Process process);

    public void StartRedirection();
    public Task StartRedirectionAsync();
    public void StopRedirection();

    public void RedirectAndWait(int timeoutMs = -1);
    public Task RedirectAndWaitAsync(int timeoutMs = -1);
}
```
