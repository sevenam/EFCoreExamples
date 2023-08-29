using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
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
  public class EfCoreBenchmarks
  {
    private readonly HRContext context;

    public EfCoreBenchmarks()
    {
      context = new HRContext();
    }


    // This will translate into: SELECT COUNT(*) FROM Employees
    [Benchmark]
    public void EmployeesCount()
    {
      context.Employees.Count();
    }

    // This will translate into: SELECT * FROM Employees --all of the columns
    // This returns all the employees, but none of this data will be used
    [Benchmark]
    public void EmployeesToListCount()
    {
      context.Employees.ToList().Count();
    }

    [Benchmark]
    public void IEnumerableEmployees()
    {
      var employees = GetEmployeesAsEnumerable();
      employees.Count();
    }

    [Benchmark]
    public void IQueryableEmployees()
    {
      var employees = GetEmployeesAsIQueryable();
      employees.Count();
    }

    // This will translate into: SELECT * FROM Employees --all of the columns
    // This returns all the employees, but none of this data will be used
    IEnumerable<Employee> GetEmployeesAsEnumerable()
    {
      return context.Employees;
    }

    // This will translate into: SELECT COUNT(*) FROM Employees
    IQueryable<Employee> GetEmployeesAsIQueryable()
    {
      return context.Employees;
    }

  }
}
