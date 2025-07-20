using System.Diagnostics;
using Servus.Core.Collections;

namespace Servus.Core.Diagnostics;

public static class ActivitySourceRegistry
{
    private record ExtendedEntry(string Name, ActivitySource Source);

    private static readonly TypeRegistry<ExtendedEntry> Registry = new();

    public static void Add<T>(string? name = null) => Create<T>(name);

    private static ExtendedEntry Create<T>(string? name = null)
    {
        var entryName = typeof(T).Name.ToSnakeCase();
        return new ExtendedEntry(entryName, new ActivitySource(entryName));
    }

    public static Activity? StartActivity<T>(string activityName, IWithTracing trace,
        ActivityKind kind = ActivityKind.Consumer)
    {
        var entry = Registry.GetOrAdd<T>(() => Create<T>());
        return trace.StartActivity(activityName, entry.Source, kind);
    }
}