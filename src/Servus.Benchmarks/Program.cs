using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;
using Servus.Benchmarks;
using Servus.Benchmarks.Collections;

var config = ManualConfig.Create(DefaultConfig.Instance)
    .WithOptions(ConfigOptions.DisableOptimizationsValidator)
    .AddExporter(MarkdownExporter.GitHub)
    .AddExporter(CsvMeasurementsExporter.Default)
    .AddExporter(RPlotExporter.Default);

BenchmarkRunner.Run<InsertAtBenchmarks>(config);
