# Line Parsing

`GetLines` returns an `IEnumerable<string>` that yields each line of a string without allocating an array like `string.Split('\n')` does. Null-safe.

## Usage

```csharp
using Servus.Core.Parsing;

string response = await httpClient.GetStringAsync(url);

foreach (var line in response.GetLines())
{
    if (line.StartsWith("#")) continue;
    Process(line);
}
```

Handles both `\n` and `\r\n` line endings, and correctly yields empty lines so you can distinguish them from the end of the input.

## API

```csharp
public static class StringExtensions  // namespace: Servus.Core.Parsing
{
    public static IEnumerable<string> GetLines(this string? value);
}
```
