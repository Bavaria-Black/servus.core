namespace Servus.Core.Diagnostics;

/// <summary>
/// Severity levels for <see cref="ServusTrace"/> events.
/// Values map directly to <see cref="Microsoft.Extensions.Logging.LogLevel"/>
/// (Trace=0, Debug=1, Information=2, Warning=3, Error=4).
/// </summary>
public enum TraceLevel : byte
{
    Trace = 0,
    Debug = 1,
    Info = 2,
    Warning = 3,
    Error = 4,
}
