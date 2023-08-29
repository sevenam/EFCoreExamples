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
  public class BenchIEnumerableVsIQueryable
  {
    private readonly HRContext context;

    public BenchIEnumerableVsIQueryable()
    {
      context = new HRContext();
    }

    [Benchmark(Baseline = true)]
    public void EmployeesCount()
    {
      context.Employees.Count(); // This will translate into: SELECT COUNT(*) FROM Employees
    }

    [Benchmark]
    public void EmployeesToListCount()
    {
      context.Employees.ToList().Count(); // This will translate into: SELECT * FROM Employees --all of the columns
    }

    [Benchmark]
    public void IEnumerableEmployees()
    {
      var employees = GetEmployeesAsEnumerable(); // This will translate into: SELECT * FROM Employees --all of the columns
      employees.Count();
    }

    [Benchmark]
    public void IQueryableEmployees()
    {
      var employees = GetEmployeesAsIQueryable(); // This will translate into: SELECT COUNT(*) FROM Employees
      employees.Count();
    }

    IEnumerable<Employee> GetEmployeesAsEnumerable()
    {
      return context.Employees;
    }

    IQueryable<Employee> GetEmployeesAsIQueryable()
    {
      return context.Employees;
    }

  }
}
