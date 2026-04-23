using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Servus.Core.Diagnostics;

/// <summary>
/// Extension methods for registering <see cref="ServusTrace"/> services with
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class ServusTraceExtensions
{
    public static IServiceCollection AddServusLoggerTracing(
        this IServiceCollection services,
        ServusTraceLevel minimumLevel = ServusTraceLevel.Debug,
        params ServusTraceCategory[] categories)
    {
        services.AddSingleton<IServusTraceListener>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var listener = new LoggerServusTraceListener(loggerFactory, minimumLevel, categories.Contains);
            ServusTrace.Configure(listener, minimumLevel, categories.Contains);
            return listener;
        });
        return services;
    }

    public static IServiceCollection AddServusLoggerTracing(
        this IServiceCollection services,
        ServusTraceLevel minimumLevel = ServusTraceLevel.Debug,
        Func<ServusTraceCategory, bool>? categoryFilter = null)
    {
        services.AddSingleton<IServusTraceListener>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var listener = new LoggerServusTraceListener(loggerFactory, minimumLevel, categoryFilter);
            ServusTrace.Configure(listener, minimumLevel, categoryFilter);
            return listener;
        });
        return services;
    }

    public static IServiceCollection AddServusTraceListener(
        this IServiceCollection services,
        IServusTraceListener listener,
        ServusTraceLevel minimumLevel = ServusTraceLevel.Debug,
        params ServusTraceCategory[] categories)
    {
        ArgumentNullException.ThrowIfNull(listener);
        ServusTrace.Configure(listener, minimumLevel, categories.Contains);
        services.AddSingleton(listener);
        return services;
    }

    public static IServiceCollection AddServusTraceListener(
        this IServiceCollection services,
        IServusTraceListener listener,
        ServusTraceLevel minimumLevel = ServusTraceLevel.Debug,
        Func<ServusTraceCategory, bool>? categoryFilter = null)
    {
        ArgumentNullException.ThrowIfNull(listener);
        ServusTrace.Configure(listener, minimumLevel, categoryFilter);
        services.AddSingleton(listener);
        return services;
    }
}