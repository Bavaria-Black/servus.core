using System.Diagnostics;
using System.Reflection;

namespace Servus.Core.Diagnostics;

/// <summary>
/// A named instrumentation channel wrapping an <see cref="ActivitySource"/>.
/// Use <see cref="SourceName"/> for the shared Servus source, or create named instances per module.
/// Extend with activity starters and semantic tag helpers via extension methods on this type.
/// Consumers subscribe via <c>AddSource("Servus")</c> (or your module name) in the OTel SDK.
/// </summary>
/// <example>
/// <code>
/// // Shared default source
/// using var activity = ServusInstrumentation.Instance.StartConnect(uri);
///
/// // Per-module source
/// private static readonly ServusInstrumentation _instr = new("MyModule");
/// using var activity = _instr.StartConnect(uri);
/// </code>
/// </example>
public class ServusInstrumentation
{
    public const string SourceName = "Servus";

    private static readonly string Version =
        typeof(ServusInstrumentation).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? typeof(ServusInstrumentation).Assembly.GetName().Version?.ToString()
        ?? "0.0.0";

    public static readonly ServusInstrumentation Instance = new(SourceName);

    public ActivitySource Source { get; }
    
    protected ServusInstrumentation(string sourceName)
    {
        Source = new ActivitySource(sourceName, Version);
    }
}