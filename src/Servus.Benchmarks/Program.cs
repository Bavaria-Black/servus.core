using BenchmarkDotNet.Running;
using Servus.Benchmarks.Events;

namespace Servus.Benchmarks
{
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<Cpu1Sub>();
            BenchmarkRunner.Run<Cpu2Sub>();
            BenchmarkRunner.Run<Cpu3Sub>();
            BenchmarkRunner.Run<Cpu5Sub>();
            BenchmarkRunner.Run<Cpu7Sub>();
            BenchmarkRunner.Run<Cpu9Sub>();
            BenchmarkRunner.Run<Cpu11Sub>();
            BenchmarkRunner.Run<Wait1Sub>();
            BenchmarkRunner.Run<Wait2Sub>();
            BenchmarkRunner.Run<Wait3Sub>();
            BenchmarkRunner.Run<Wait5Sub>();
            BenchmarkRunner.Run<Wait7Sub>();
            BenchmarkRunner.Run<Wait9Sub>();
            BenchmarkRunner.Run<Wait11Sub>();
            
        }
    }
}