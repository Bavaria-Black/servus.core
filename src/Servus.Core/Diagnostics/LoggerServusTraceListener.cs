using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Servus.Core.Diagnostics;

/// <summary>
/// Routes <see cref="ServusTraceEvent"/> instances to <see cref="ILoggerFactory"/>,
/// creating one <see cref="ILogger"/> per <see cref="ServusTraceCategory"/> on demand.
/// Logger names follow the pattern <c>Servus.Trace.{Category.Name}</c>.
/// </summary>
internal sealed class LoggerServusTraceListener : IServusTraceListener
{
    private readonly ConcurrentDictionary<ServusTraceCategory, ILogger> _loggers = new();
    private readonly ILoggerFactory _loggerFactory;
    private readonly Func<ServusTraceCategory, bool>? _enabledCategories;
    private readonly ServusTraceLevel _minimumLevel;

    public LoggerServusTraceListener(
        ILoggerFactory loggerFactory,
        ServusTraceLevel minimumLevel = ServusTraceLevel.Debug,
        Func<ServusTraceCategory, bool>? categoryFilter = null)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        _loggerFactory = loggerFactory;
        _minimumLevel = minimumLevel;
        _enabledCategories = categoryFilter;
    }

    /// <inheritdoc />
    public bool IsEnabled(ServusTraceLevel level, ServusTraceCategory category)
    {
        return level >= _minimumLevel && (_enabledCategories is null || _enabledCategories.Invoke(category));
    }

    /// <inheritdoc />
    public void Write(in ServusTraceEvent evt)
    {
        var logLevel = (LogLevel)evt.Level;
        var logger = _loggers.GetOrAdd(evt.Category,
            c => _loggerFactory.CreateLogger($"Servus.Trace.{c.Name}"));
        if (!logger.IsEnabled(logLevel)) return;
        var message = evt.FormatMessage();
        logger.Log(logLevel, "[{SourceType}#{SourceHash:X8}] {Message}",
            evt.SourceType, evt.SourceHash, message);
    }
}