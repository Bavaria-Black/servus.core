namespace Servus.Core.Diagnostics;

/// <summary>
/// Receives trace events from <see cref="ServusTrace"/>.
/// Implementations must be thread-safe.
/// </summary>
public interface IServusTraceListener
{
    /// <summary>
    /// Returns <see langword="true"/> when this listener wants events
    /// at the given <paramref name="level"/> in the given <paramref name="category"/>.
    /// Called inside <see cref="ServusTrace.ShouldTrace"/> on the hot path.
    /// </summary>
    bool IsEnabled(TraceLevel level, string category);

    /// <summary>
    /// Receives a single trace event. Called only when
    /// <see cref="IsEnabled"/> returned <see langword="true"/>.
    /// </summary>
    void Write(in TraceEvent evt);
}
