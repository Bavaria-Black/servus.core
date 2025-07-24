# Benchmark Dot Net

Execute the `Program.Main` console application to run the benchmarks.

You can find the documentation under:
https://github.com/dotnet/BenchmarkDotNet
https://benchmarkdotnet.org/

# Setup R for Plots
The attribute `[RPlotExporter]` exporter creates nice plots using R and ggplot 2.

![image.png](https://dirnhofer.visualstudio.com/91b65d76-3515-4e16-b111-437cb4e62822/_apis/git/repositories/7490db1f-12e4-4305-9c67-1a38831694c3/pullRequests/18/attachments/image.png) 

## To setup R:

Install [R open](https://chocolatey.org/packages/microsoft-r-open)
` winget install -e --id RProject.R`

Create a system environment variable with the path to the R installation called R_HOME. Use the root of the R distribution and not the bin folder. For example: `R_HOME=C:\Program Files\Microsoft\R Open\R-3.5.3`. 

And add `C:\Program Files\Microsoft\R Open\R-3.5.3\bin` to the `PATH` environment variable. Remember to use the bin folder. It should contain Rscript.exe