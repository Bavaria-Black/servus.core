using System.Diagnostics;
using System.Reflection;
using Servus.Core.Collections;
using Servus.Core.Text;

namespace Servus.Core.Diagnostics;

public static class ActivitySourceRegistry
{
    private sealed record ExtendedEntry(string Name, ActivitySource Source);

    private static readonly TypeRegistry<ExtendedEntry> Registry = new();
    private static readonly TypeRegistry<Type> NameRegistry = new();

    public static void Add<T>(string? name = null) => Create<T>(name);
    public static void Add(Type key, string? name = null) => Create(key, name);

    private static ExtendedEntry Create<T>(string? name = null) => Create(typeof(T), name);
    private static ExtendedEntry Create(Type key, string? name = null)
    {
        var targetType = NameRegistry.GetOrAdd(key, () => key.GetCustomAttribute<ActivitySourceKeyAttribute>()
            ?.SourceKey ?? key);

        var entryName = name
                        ?? targetType.GetCustomAttribute<ActivitySourceNameAttribute>()
                            ?.ActivitySourceName
                        ?? targetType.Name.ToSnakeCase();
        
        return new ExtendedEntry(entryName, new ActivitySource(entryName));
    }

    public static Activity? StartActivity<T>(string activityName, IWithTracing trace,
        ActivityKind kind = ActivityKind.Consumer) => StartActivity(typeof(T), activityName, trace, kind);

    public static Activity? StartActivity(Type key, string activityName, IWithTracing trace,
        ActivityKind kind = ActivityKind.Consumer)
    {
        var entry = Registry.GetOrAdd(key,() => Create(key));
        return trace.StartActivity(activityName, entry.Source, kind);
    }
}