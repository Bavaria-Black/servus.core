using BenchmarkDotNet.Running;
using Servus.Benchmarks.Events;

namespace Servus.Benchmarks
{
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<EventBusBenchmarks>();
        }
    }
}