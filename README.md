# EFCoreExamples

## BenchmarkDotNet

First, install R and add it to the path (e.g.: C:\Program Files\R\R-4.3.1\bin):

```bash
winget install -e --id RProject.R
```

Make sure to restart Visual Studio after editing the path environment variable.

Also make sure to build for Release and not Debug and Run without Debugging (Ctrl+F5).

To run against multiple .NET versions, update csproj file to include all the versions:

```xml
<TargetFrameworks>net48;net7.0;net6.0;netcoreapp3.1</TargetFrameworks>
```

and in the benchmark file add the wanted veresions as SimpleJobs():

```cs
  [SimpleJob(RuntimeMoniker.Net60)]
  [SimpleJob(RuntimeMoniker.Net70)]
  [RPlotExporter] // generates plots using R
  public class EfCoreBenchmarks
```

Methods to be benchmarked must be marked with the [Benchmark] attribute, like this:

```cs
    [Benchmark]
    public void ToListCount()
    {
      using var context = new HRContext();
      context.Employees.ToList().Count();
    }
```