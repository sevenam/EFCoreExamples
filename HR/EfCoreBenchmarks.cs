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
  [RPlotExporter]
  public class EfCoreBenchmarks
  {

    [Benchmark]
    public void ToListCount()
    {
      using var context = new HRContext();
      context.Employees.ToList().Count();
    }

    [Benchmark]
    public void ListEmployees()
    {
      using var context = new HRContext();
      context.Employees.Count();
    }

    [Benchmark]
    public void IEnumerableEmployees()
    {
      GetEmployeesAsEnumerable();
    }

    [Benchmark]
    public void IQueryableEmployees()
    {
      GetEmployeesAsIQueryable();
    }

    // This will translate into: SELECT * FROM Employees
    IEnumerable<Employee> GetEmployeesAsEnumerable()
    {
      using var context = new HRContext();
      return context.Employees;
    }

    IQueryable<Employee> GetEmployeesAsIQueryable()
    {
      using var context = new HRContext();
      return context.Employees;
    }

  }
}
