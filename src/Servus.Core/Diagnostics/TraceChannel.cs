using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Servus.Core.Diagnostics;

/// <summary>
/// A trace channel bound to a single category.
/// Obtain channels with <see cref="ServusTrace.For(string)"/>.
/// </summary>
public readonly struct TraceChannel(string category)
{
    public void Trace(object source, string message, params object?[] args)
        => Servus.Tracing.Trace(TraceLevel.Trace, category, source, message, null, args);

    public void Debug(object source, string message, params object?[] args)
        => Servus.Tracing.Trace(TraceLevel.Debug, category, source, message, null, args);

    public void Info(object source, string message, params object?[] args)
        => Servus.Tracing.Trace(TraceLevel.Info, category, source, message, null, args);

    public void Warning(object source, string message, params object?[] args)
        => Servus.Tracing.Trace(TraceLevel.Warning, category, source, message, null, args);

    public void Error(object source, string message, params object?[] args)
        => Servus.Tracing.Trace(TraceLevel.Error, category, source, message, null, args);
}
