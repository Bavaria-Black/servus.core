using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace Servus.Benchmarks;

[Config(typeof(Config))]
[RPlotExporter]
public class Md5VsSha256
{
    private const int N = 10000;
    private readonly byte[] _data;

    private readonly SHA256 _sha256 = SHA256.Create();
    private readonly MD5 _md5 = MD5.Create();

    public Md5VsSha256()
    {
        _data = new byte[N];
        const int seed = (5 + 5 + 5) * (5 * 5 + 5 + 5 + (int) (0.5 + 0.5 + 0.5 + 0.5));
        new Random(seed).NextBytes(_data);
    }

    [Benchmark]
    public byte[] Sha256() => _sha256.ComputeHash(_data);

    [Benchmark]
    public byte[] Md5() => _md5.ComputeHash(_data);
}