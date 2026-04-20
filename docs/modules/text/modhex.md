# ModHex Encoding

`ModHexEncoding` is the modified-hex encoding used by Yubikey and similar hardware: it maps the nibbles `0-F` to the letters `c, b, d, e, f, g, h, i, j, k, l, n, r, t, u, v` so the output is always a valid US-English keyboard sequence.

It derives from `System.Text.Encoding`, so anywhere an `Encoding` is accepted will take it.

## Usage

```csharp
using Servus.Core.Encoding;

// Singleton instance
var encoding = ModHexEncoding.ModHex;

string modhex = ModHexEncoding.ConvertFromAscii("servus");   // "ifjjjk..."
string ascii  = ModHexEncoding.ConvertToAscii(modhex);       // "servus"
```

## As a standard `Encoding`

```csharp
byte[] bytes = ModHexEncoding.ModHex.GetBytes("cbcbcbcb");
string text  = ModHexEncoding.ModHex.GetString(bytes);
```

## API

```csharp
public class ModHexEncoding : System.Text.Encoding
{
    public static ModHexEncoding ModHex { get; }

    public static string ConvertFromAscii(string ascii);
    public static string ConvertToAscii(string modHex);

    // Plus standard Encoding overrides:
    //   GetByteCount, GetBytes, GetCharCount, GetChars,
    //   GetMaxByteCount, GetMaxCharCount
}
```

## When you'd reach for this

- Parsing or generating Yubikey OTPs (the [Yubikey validator](/modules/utilities/yubikey) uses this under the hood).
- Any hardware that emits data as US-English keystrokes to avoid keyboard-layout issues.
