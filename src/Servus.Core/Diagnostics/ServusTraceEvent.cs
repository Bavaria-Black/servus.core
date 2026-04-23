using System.Diagnostics;

namespace Servus.Core.Diagnostics;

/// <summary>
/// Immutable trace event with deferred message formatting.
/// Stores the template and arguments; <see cref="FormatMessage"/>
/// allocates a formatted string only when called.
/// </summary>
public readonly struct ServusTraceEvent
{
    /// <summary>Timestamp from <see cref="Stopwatch.GetTimestamp"/>.</summary>
    public long TimestampTicks { get; }

    /// <summary>Severity level of this event.</summary>
    public ServusTraceLevel Level { get; }

    /// <summary>Category that produced this event.</summary>
    public ServusTraceCategory Category { get; }

    /// <summary>Short type name of the source object (from <c>GetType().Name</c>).</summary>
    public string SourceType { get; }

    /// <summary>Identity hash of the source object (from <c>GetHashCode()</c>).</summary>
    public int SourceHash { get; }

    /// <summary>Format template (compatible with <see cref="string.Format(string,object?)"/>).</summary>
    public string Template { get; }

    private readonly object?[] _args;

    internal ServusTraceEvent(
        long timestampTicks,
        ServusTraceLevel level,
        ServusTraceCategory category,
        string sourceType,
        int sourceHash,
        string template,
        params object?[] args)
    {
        TimestampTicks = timestampTicks;
        Level = level;
        Category = category;
        SourceType = sourceType;
        SourceHash = sourceHash;
        Template = template;
        _args = args;
    }

    /// <summary>
    /// Formats the message by applying stored arguments to the template.
    /// This is the only method that allocates a string.
    /// </summary>
    public string FormatMessage()
    {
        if (_args.Length == 0)
        {
            return Template;
        }

        try
        {
            return string.Format(Template, args: _args);
        }
        catch (FormatException)
        {
            return Template;
        }
    }
}