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
  public class BenchUpdate
  {
    private readonly HRContext context;

    public BenchUpdate()
    {
      context = new HRContext();
    }

    [Benchmark(Baseline = true)]
    public void UpdateSwapNames()
    {
      var result = context.Employees.TagWith("Fetching all of the employees!").ExecuteUpdate(x => x
        .SetProperty(y => y.FirstName, y => y.LastName)
        .SetProperty(y => y.LastName, y => y.FirstName));
      context.SaveChanges();
    }

    [Benchmark]
    public void UpdateSwapNamesSelectFirst()
    {
      var result = context.Employees.TagWith(nameof(UpdateSwapNamesSelectFirst)).ToList();
      foreach(var employee in result)
      {
        employee.FirstName = employee.LastName;
        employee.LastName = employee.FirstName;
      }
      context.SaveChanges(); // will generate an UPDATE statement for each employee
    }


  }
}
