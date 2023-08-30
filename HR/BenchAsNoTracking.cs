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
  public class BenchAsNoTracking
  {
    private readonly HRContext context;

    public BenchAsNoTracking()
    {
      context = new HRContext();
    }

    [Benchmark(Baseline = true)]
    public void Employees()
    {
      context.Employees.Include(x => x.Skills).Include(x => x.Department).ToList();
    }

    [Benchmark]
    public void EmployeeAsNoTracking()
    {
      context.Employees.Include(x => x.Skills).Include(x => x.Department).AsNoTracking().ToList();
    }


  }
}
