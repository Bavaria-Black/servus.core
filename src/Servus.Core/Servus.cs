using Servus.Core.Diagnostics;

namespace Servus.Core;

public static class Servus
{
    public static readonly ServusTrace Tracing = new();
    public static readonly ServusMetrics Metrics = new();
}