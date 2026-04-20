# Key-Value Parsing

`LineKeyValueParser` parses text where each line is a `key=value` pair (or any other single-character separator, or a regex). Typical uses: config files, `.env`-style formats, INI-ish outputs from CLI tools.

## Usage

```csharp
using Servus.Core.Parsing.Text;

var parser = new LineKeyValueParser('=');

string envFile = """
    APP_NAME=Servus
    APP_ENV=production
    LOG_LEVEL=info
    """;

foreach (var kv in parser.Parse(envFile))
{
    Console.WriteLine($"{kv.Key} -> {kv.Value}");
}
```

## Custom separator

Any single char works as a separator:

```csharp
var colon = new LineKeyValueParser(':');
colon.Parse("name: Andreas\nage: 42");
```

## Regex-based split

For more complex cases (e.g. "split on `=` or `:`, trim surrounding whitespace"), pass a regex:

```csharp
var flexible = new LineKeyValueParser(@"\s*[=:]\s*");

foreach (var kv in flexible.Parse("name : Andreas\nage=42"))
{
    // ("name", "Andreas"), ("age", "42")
}
```

## Single-line parsing

If you already have a single line, skip the enumeration:

```csharp
var kv = parser.ParseLine("APP_NAME=Servus");
// kv.Key = "APP_NAME", kv.Value = "Servus"
```

## API

```csharp
public class LineKeyValueParser
{
    public LineKeyValueParser(char splitChar);
    public LineKeyValueParser(string splitRegex);

    public IEnumerable<KeyValuePair<string, string>> Parse(string value);
    public KeyValuePair<string, string> ParseLine(string value);
}
```
