using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR
{

  //[SimpleJob(RuntimeMoniker.Net60)]
  [SimpleJob(RuntimeMoniker.Net70)]
  [MemoryDiagnoser(false)] // include memory allocation results
  [MinIterationCount(1)] // min number of iterations
  [MaxIterationCount(2)] // max number of iterations
  [RankColumn] // rank the results
  [RPlotExporter] // plot results with R
  [MeanColumn] // mean of the results
  [BaselineColumn]
  public class BenchExplicitIncludes
  {
    private readonly HRContext context;

    public BenchExplicitIncludes()
    {
      context = new HRContext();
    }

    [Benchmark(Baseline = true)]
    public void EmployeesImplicitInclude()
    {
      var result = context.Employees.Select(x => new { x.FirstName, x.LastName, x.Department.Name }).ToList();
    }

    [Benchmark]
    public void EmployeesExplicitInclude()
    {
      var result = context.Employees.Include(x => x.Department).Select(y => new { y.FirstName, y.LastName, y.Department.Name }).ToList();
    }


  }
}
