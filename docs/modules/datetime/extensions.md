# DateTime Extensions

Small predicate helpers on `DateTime`. Each one is a one-liner you'd otherwise re-write per project.

```csharp
using Servus.Core;

var dt = DateTime.UtcNow;

dt.IsToday();                               // same date as "today"
dt.IsBetween(startDate, endDate);           // start <= dt <= end (bounds auto-swap)
dt.IsWorkday();                             // Monday..Friday
dt.IsWeekend();                             // Saturday or Sunday
dt.IsInFuture();                            // compared against UtcNow
dt.IsPast();                                // compared against UtcNow
```

`IsBetween` auto-swaps the bounds, so `IsBetween(end, start)` gives the same result as `IsBetween(start, end)`.

## API

```csharp
public static class DateTimeExtension
{
    public static bool IsToday(this DateTime dateTime);
    public static bool IsBetween(this DateTime dateTime, DateTime lowerBound, DateTime upperBound);
    public static bool IsWorkday(this DateTime dateTime);
    public static bool IsWeekend(this DateTime dateTime);
    public static bool IsInFuture(this DateTime dateTime);
    public static bool IsPast(this DateTime dateTime);
}
```
