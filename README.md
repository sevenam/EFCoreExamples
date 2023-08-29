# EFCoreExamples

References:
- Source: Common mistakes in EF Core - Jernej Kavka - NDC Oslo 2023
- Youtube: https://www.youtube.com/watch?v=dDANjr5MCew
- This code/repo: https://github.com/sevenam/EFCoreExamples

## #1 IEnumerable vs IQueryable (benchmark)

Methods returning IEnumerable could translate into:
```sql
SELECT * FROM Employees --all of the columns
```

While methods returning IQueryable could translate into: 
```sql
SELECT COUNT(*) FROM Employees
```

## #2 AsNoTracking() (benchmark)

- EF tracks entities in case you do `SaveChanges()` on the context
- If you use `AsNoTracking()` it will not track
- Can be used when only selecting data (without doing any changes to the data)
- Performance wise, database provider could matter (sqlite/mssql/postgres/etc)



## #3 Explicit Includes (benchmark)

- When using `Include()` it will explicitly force inclusion of data even though it is never used
- Even if you use `Select()` the whole entity in the `Include()` call will still be included

## #4 Pagination

- Mostly the same as #1
- Make sure to use IQueryable for the query, so you can use it to both:
1. `query.CountAsync()` to get the total count (and as it is IQueryable it will not get all the data)
2. `Skip()` and `Take()` to get the page
_
## #5 Non-Cancellable Queries

- Some queries are just heavy...
- If the user cancels the HTTP request in the app the query will still run on the db server unless cancellation tokens are used
- Scenario: query takes 30 seconds to run and user spams F5 to refresh the page constantly...
- Solution: pass the cancellation token to the db query

```cs
[HttpGet]
public async Task<IActionResult> GetEmployees(CancellationToken ct)
{
 result = await dbContext.Employees.ToListAsync(ct);
 return result;
}
```

## #6 Inefficient update/delete (benchmark)

- Before EF7 - we had to load all the entities we wanted to change first
- But now; update directly on the db without fetching first

## Bonus #1

- Use DbContextPool to re-use existing DbContext instances (and spin up fewer new ones)
- Improves resiliency and performance (e.g. F5 spamming will reuse the same DbContext instance)

```cs
services.AddDbContextPool<DbContext>(options =>
{
 options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
})
```

## Bonus #2

- `TagWith()` for simpler debugging
- Maybe even tag with class name and method name

```cs
var result = context.Employees.TagWith("Fetching all of the employees!").ToList();
```



## BenchmarkDotNet

Add BenchmarkDotNet NuGet package to the csproj file:

```xml
<PackageReference Include="BenchmarkDotNet" Version="0.13.7" />
```

Install R and add it to the path (e.g.: C:\Program Files\R\R-4.3.1\bin):

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
  [MemoryDiagnoser(true)] // include memory allocation results
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