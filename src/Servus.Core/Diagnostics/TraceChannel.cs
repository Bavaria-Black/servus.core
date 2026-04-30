using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Servus.Core.Diagnostics;

/// <summary>
/// A trace channel bound to a single category.
/// Obtain channels with <see cref="ServusTrace.For(string)"/>.
/// </summary>
public readonly struct TraceChannel(string category)
{
    public void Trace<T>(T source, string message, params object?[] args)
        => Servus.Tracing.Trace(source, TraceLevel.Trace, category, message, null, args);

    public void Debug<T>(T source, string message, params object?[] args)
        => Servus.Tracing.Trace(source, TraceLevel.Debug, category, message, null, args);

    public void Info<T>(T source, string message, params object?[] args)
        => Servus.Tracing.Trace(source, TraceLevel.Info, category, message, null, args);

    public void Warning<T>(T source, string message, params object?[] args)
        => Servus.Tracing.Trace(source, TraceLevel.Warning, category, message, null, args);

    public void Error<T>(T source, string message, params object?[] args)
        => Servus.Tracing.Trace(source, TraceLevel.Error, category, message, null, args);
}
