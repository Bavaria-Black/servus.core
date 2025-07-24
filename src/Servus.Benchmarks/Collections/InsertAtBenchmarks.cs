using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Servus.Core;

namespace Servus.Benchmarks.Collections;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
[RankColumn]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[CsvMeasurementsExporter]
[RPlotExporter]
public class InsertAtBenchmarks
{
    private int[] _mediumArray;
    private List<int> _mediumList;
    private int[] _insert;
    private IEnumerable<int> _mediumEnumerable;

    [IterationSetup]
    public void Setup()
    {
        _insert = Enumerable.Range(0, 100).ToArray();

        // Medium collections (1,000 elements)
        _mediumArray = Enumerable.Range(1, 10000).ToArray();
        _mediumList = Enumerable.Range(1, 10000).ToList();
        _mediumEnumerable = Enumerable.Range(1, 10000);
    }

    // ===================== COMPARISON WITH NAIVE APPROACHES =====================

    [BenchmarkCategory("Single")]
    [Benchmark(Baseline = true)]
    public int[] Base()
    {
        _mediumList.Insert(500, 1);
        return _mediumList.ToArray();
    }

    [BenchmarkCategory("Single")]
    [Benchmark]
    public int[] BA1()
    {
        return _mediumArray.InsertAt(500, 1);
    }

    [BenchmarkCategory("Single")]
    [Benchmark]
    public int[] BL1()
    {
        return _mediumList.InsertAt(500, 1).ToArray();
    }

    [BenchmarkCategory("Single")]
    [Benchmark]
    public int[] BE1()
    {
        return _mediumEnumerable.InsertAt(500, 1).ToArray();
    }
}