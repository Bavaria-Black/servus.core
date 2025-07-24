using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BenchmarkDotNet.Attributes;
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

        string csvFullPath = CsvMeasurementsExporter.Default.GetArtifactFullName(summary);
        string scriptFullPath = Path.Combine(summary.ResultsDirectoryPath, scriptFileName);

        if (!TryFindRScript(consoleLogger, out string rscriptPath))
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
        string rscriptExecutable = "Rscript.exe";
        rscriptPath = null;

        string rHome = Environment.GetEnvironmentVariable("R_HOME");
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
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFilesR = Path.Combine(programFiles, "R");
            if (Directory.Exists(programFilesR))
            {
                foreach (string rRootDirectory in Directory.EnumerateDirectories(programFilesR))
                {
                    string rscriptPathCandidate = Path.Combine(rRootDirectory, "bin", rscriptExecutable);
                    if (File.Exists(rscriptPathCandidate))
                    {
                        rscriptPath = rscriptPathCandidate;
                        return true;
                    }
                }
            }
        }

        consoleLogger.WriteLineError(
            $"{nameof(RPlotExporter)} couldn't find {rscriptExecutable} in your PATH and no R_HOME environment variable is defined");
        return false;
    }

    private static string? FindInPath(string fileName)
    {
        string path = Environment.GetEnvironmentVariable("PATH");
        if (path == null)
            return null;

        string[] dirs = path.Split(Path.PathSeparator);
        foreach (string dir in dirs)
        {
            string trimmedDir = dir.Trim('\'', '"');
            try
            {
                string filePath = Path.Combine(trimmedDir, fileName);
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

public class RPlotExporterExtendedAttribute() : ExporterConfigBaseAttribute(RPlotExporterExtended.Default);