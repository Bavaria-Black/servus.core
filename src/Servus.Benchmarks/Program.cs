using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Servus.Benchmarks
{
    static class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Md5VsSha256>();
        }
    }

    [Config(typeof(Config))]
    [RPlotExporter]
    public class Md5VsSha256
    {
        private const int N = 10000;
        private readonly byte[] data;

        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();

        public Md5VsSha256()
        {
            data = new byte[N];
            const int seed = (5 + 5 + 5) * (5 * 5 + 5 + 5 + (int) (0.5 + 0.5 + 0.5 + 0.5));
            new Random(seed).NextBytes(data);
        }

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);
    }
}