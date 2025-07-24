using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using Servus.Core.Application.Console;

namespace Servus.Benchmarks;

public class RPlotExporterExtended : IExporter
{
    public static readonly IExporter Default = new RPlotExporterExtended();
    public string Name => nameof(RPlotExporterExtended);

    public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
    {
        const string scriptFileName = "BuildPlots.R";
        yield return Path.Combine(summary.ResultsDirectoryPath, scriptFileName);

        var csvFullPath = CsvMeasurementsExporter.Default.GetArtifactFullName(summary);
        var scriptFullPath = Path.Combine(summary.ResultsDirectoryPath, scriptFileName);

        if (!TryFindRScript(consoleLogger, out var rscriptPath))
        {
            yield break;
        }

        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            FileName = rscriptPath,
            WorkingDirectory = summary.ResultsDirectoryPath,
            Arguments = $"\"{scriptFullPath}\" \"{csvFullPath}\""
        };
        
        using var redirector = new ProcessOutRedirector(process);
        process.Start();
        redirector.StartRedirection();
        
        process.StartInfo.RedirectStandardInput = true;
        process.StandardInput.WriteLine(10);
        process.StandardInput.Flush();
        process.StandardInput.WriteLine(5);
        process.StandardInput.Flush();
        
        process.WaitForExit();
        redirector.StopRedirection();
        
        
        yield return Path.Combine(summary.ResultsDirectoryPath, $"*.png");
    }

    public void ExportToLog(Summary summary, ILogger logger)
    {
        throw new NotSupportedException();
    }

    private static bool TryFindRScript(ILogger consoleLogger, out string rscriptPath)
    {
        var rscriptExecutable = "Rscript.exe";
        rscriptPath = null;

        var rHome = Environment.GetEnvironmentVariable("R_HOME");
        if (rHome != null)
        {
            rscriptPath = Path.Combine(rHome, "bin", rscriptExecutable);
            if (File.Exists(rscriptPath))
                return true;

            consoleLogger.WriteLineError(
                $"{nameof(RPlotExporter)} requires R_HOME to point to the parent directory of the existing '{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}{rscriptExecutable} (currently points to {rHome})");
            return false;
        }

        // No R_HOME, or R_HOME points to a wrong folder, try the path
        rscriptPath = FindInPath(rscriptExecutable);
        if (rscriptPath != null)
            return true;

        if (true)
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var programFilesR = Path.Combine(programFiles, "R");
            if (Directory.Exists(programFilesR))
            {
                foreach (var rRootDirectory in Directory.EnumerateDirectories(programFilesR))
                {
                    var rscriptPathCandidate = Path.Combine(rRootDirectory, "bin", rscriptExecutable);
                    if (!File.Exists(rscriptPathCandidate)) continue;
                    
                    rscriptPath = rscriptPathCandidate;
                    return true;
                }
            }
        }

        consoleLogger.WriteLineError(
            $"{nameof(RPlotExporter)} couldn't find {rscriptExecutable} in your PATH and no R_HOME environment variable is defined");
        return false;
    }

    private static string FindInPath(string fileName)
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        if (path == null)
            return null;

        var dirs = path.Split(Path.PathSeparator);
        foreach (var dir in dirs)
        {
            var trimmedDir = dir.Trim('\'', '"');
            try
            {
                var filePath = Path.Combine(trimmedDir, fileName);
                if (File.Exists(filePath))
                    return filePath;
            }
            catch (Exception)
            {
                // Never mind
            }
        }

        return null;
    }
}   