using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;

namespace Servus.Benchmarks
{
    public class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(CsvMeasurementsExporter.Default);
            AddExporter(RPlotExporter.Default);
        }
    }
}