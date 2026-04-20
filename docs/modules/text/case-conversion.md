# Case Conversion

Three extensions on `string` that convert from the typical .NET identifier (PascalCase / camelCase) into the three most-asked-for styles:

```csharp
using Servus.Core.Text;

"UserAccountId".ToSnakeCase();  // "user_account_id"
"UserAccountId".ToKebabCase();  // "user-account-id"
"UserAccountId".ToDotCase();    // "user.account.id"
```

They also survive mixed input shapes:

```csharp
"userAccount_ID".ToSnakeCase();  // "user_account_id"
"HTTP2Server".ToKebabCase();     // "http2-server"
"DOMParser".ToDotCase();         // "dom.parser"
```

## API

```csharp
public static partial class StringExtensions  // namespace: Servus.Core.Text
{
    public static string ToSnakeCase(this string value);
    public static string ToKebabCase(this string value);
    public static string ToDotCase(this string value);
}
```

## Use cases

- Serialising .NET types into JSON/YAML keys with non-PascalCase conventions.
- Generating filenames, URL slugs, config keys from .NET identifiers.
- Auto-deriving ActivitySource names from type names (see [Tracing](/modules/diagnostics/tracing)).
