using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Servus.Core.Diagnostics;

/// <summary>
/// Routes <see cref="TraceEvent"/> instances to <see cref="ILoggerFactory"/>,
/// creating one <see cref="ILogger"/> per category on demand.
/// Logger names follow the pattern <c>Servus.Trace.{Category}</c>.
/// </summary>
internal sealed class TraceLogger : IServusTraceListener
{
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();
    private readonly ILoggerFactory _loggerFactory;
    private readonly Func<string, bool>? _enabledCategories;
    private readonly TraceLevel _minimumLevel;

    public TraceLogger(
        ILoggerFactory loggerFactory,
        TraceLevel minimumLevel = TraceLevel.Debug,
        Func<string, bool>? categoryFilter = null)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        _loggerFactory = loggerFactory;
        _minimumLevel = minimumLevel;
        _enabledCategories = categoryFilter;
    }

    /// <inheritdoc />
    public bool IsEnabled(TraceLevel level, string category)
    {
        return level >= _minimumLevel && (_enabledCategories is null || _enabledCategories.Invoke(category));
    }

    /// <inheritdoc />
    public void Write(in TraceEvent evt)
    {
        var logLevel = (LogLevel)evt.Level;
        var logger = _loggers.GetOrAdd(evt.Category,
            c => _loggerFactory.CreateLogger($"Servus.Trace.{c}"));
        if (!logger.IsEnabled(logLevel)) return;
        var message = evt.FormatMessage();
        logger.Log(logLevel, "[{SourceType}#{SourceHash:X8}] {Message}",
            evt.SourceType, evt.SourceHash, message);
    }
}
