using System.Diagnostics;
using System.Reflection;
using Servus.Core.Collections;

namespace Servus.Core.Diagnostics;

public static class ActivitySourceRegistry
{
    private record ExtendedEntry(string Name, ActivitySource Source);

    private static readonly TypeRegistry<ExtendedEntry> Registry = new();
    private static readonly TypeRegistry<Type> NameRegistry = new();

    public static void Add<T>(string? name = null) => Create<T>(name);

    private static ExtendedEntry Create<T>(string? name = null)
    {
        var type = typeof(T);
        var targetType = NameRegistry.GetOrAdd<T>(() => type.GetCustomAttribute<ActivitySourceKeyAttribute>()
            ?.SourceKey ?? type);

        var entryName = name
                        ?? targetType.GetCustomAttribute<ActivitySourceNameAttribute>()
                            ?.ActivitySourceName
                        ?? targetType.Name.ToSnakeCase();
        
        return new ExtendedEntry(entryName, new ActivitySource(entryName));
    }


    public static Activity? StartActivity<T>(string activityName, IWithTracing trace,
        ActivityKind kind = ActivityKind.Consumer)
    {
        var entry = Registry.GetOrAdd<T>(() => Create<T>());
        return trace.StartActivity(activityName, entry.Source, kind);
    }
}