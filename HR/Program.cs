using BenchmarkDotNet.Running;
using HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

static List<Employee> ReadCSV(string filePath)
{
    var employees = new List<Employee>();
    var skills = new List<Skill>();
    var departments = new List<Department>();
    var profiles = new List<EmployeeProfile>();

    using var reader = new StreamReader(filePath);

    // Read the header line
    var headerLine = reader.ReadLine();

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        if (line == null)
        {
            break;
        }

        // Split the line by comma
        var values = line.Split(',');

        if (values.Length == 8)
        {
            var firstName = values[0];
            var lastName = values[1];
            var salary = values[2];
            var joinedDate = DateTime.ParseExact(values[3], "M/d/yyyy", null);
            var phone = values[4];
            var email = values[5];
            var departmentName = values[6];
            var skillTitles = values[7].Split(';');

            // Create Department object if it doesn't exist
            var department = departments.Find(d => d.Name == departmentName);
            if (department == null)
            {
                department = new Department
                {
                    Name = departmentName
                };
                departments.Add(department);
            }

            // Create EmployeeProfile object
            var profile = new EmployeeProfile
            {
                Phone = phone,
                Email = email
            };
            profiles.Add(profile);

            // Create Employee object
            var employee = new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                Salary = decimal.Parse(salary),
                JoinedDate = joinedDate,
                Department = department,
                Profile = profile
            };
            employees.Add(employee);

            // Create Skill objects
            foreach (var skillTitle in skillTitles)
            {
                var skill = skills.Find(s => s.Title == skillTitle);
                if (skill == null)
                {
                    skill = new Skill
                    {
                        Title = skillTitle
                    };
                    skills.Add(skill);
                }

                // Add skill to the employee's Skills collection
                employee.Skills.Add(skill);
            }
        }

    }

    return employees;
}



// seed data:
//var employees = ReadCSV("data.csv");

//Console.WriteLine($"{employees.Count} row(s) found");

//using var context = new HRContext();
//foreach (var employee in employees)
//{
//    context.Add(employee);
//}

//context.SaveChanges();




//debugging:
//var noTracking = new BenchAsNoTracking();
//noTracking.EmployeeAsNoTracking();
//var explicitIncludes = new BenchExplicitIncludes();
//explicitIncludes.EmployeesImplicitInclude();
//explicitIncludes.EmployeesExplicitInclude();





// References: https://www.youtube.com/watch?v=dDANjr5MCew
// This code/repo: https://github.com/sevenam/EFCoreExamples


// #1 IEnumerable vs IQueryable
BenchmarkRunner.Run<BenchIEnumerableVsIQueryable>();

// #2 AsNoTracking: tracks entities in case you do SaveChanges() on the context - can be used when you only want to read data
//BenchmarkRunner.Run<BenchAsNoTracking>(); // database provider could matter (sqlite/mssql/postgres/etc)

// #3 ExplicitIncludes: The includes will explicitly force inclusion of data even though it is never used
//BenchmarkRunner.Run<BenchExplicitIncludes>(); // even if you use select the whole included entity will still be included


// #4 Pagination: Mostly the same as #1. Make sure to use IQueryable for the query, so you can use it to both:
//     - do query.CountAsync() to get the total count (and as it is IQueryable it will not get all the data)
//     - do Skip() and Take() to get the page

// #5 (Non)-Cancellable queries: Some queries are just heavy, and if the user cancels the HTTP request in the app
//    the query will still run on the db server unless cancellation tokens are used
//    scenario: query takes 30 seconds to run and user spams F5 to refresh the page constantly...
//    solution: pass the cancellation token to the db query in e.g. ToListAsync(ct);
//    example:

//[HttpGet]
//public async Task<IActionResult> GetEmployees(CancellationToken cancellationToken)
//{
//  result = await dbContext.Employees.ToListAsync(cancellationToken);
//  return result;
//}

// #6 Inefficient update/delete
// Before EF7 - we had to load all the entities we wanted to change first, but now: update directly on the db without fetching first
//BenchmarkRunner.Run<BenchUpdate>();

// Bonus: Use DbContextPool to re-use existing DbContext instances (and spin up fewer new ones)
// Improves resiliency and performance (e.g. F5 spamming will reuse the same DbContext instance)
// example:

//services.AddDbContextPool<DbContext>(options =>
//{
//  options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
//})

// Bonus: TagWith() for simpler debugging (maybe even tag with class name and method name)
// example:

// var result = context.Employees.TagWith("Fetching all of the employees!").ToList();