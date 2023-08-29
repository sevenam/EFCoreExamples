using BenchmarkDotNet.Running;
using HR;

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




// debugging:
//var bm = new EfCoreBenchmarks();
//bm.IQueryableEmployees();
//bm.IEnumerableEmployees();






// #1 IEnumerable vs IQueryable
//BenchmarkRunner.Run<BenchIEnumerableVsIQueryable>();

// #2 AsNoTracking: tracks entities in case you do SaveChanges() on the context - can be used when you only want to read data
BenchmarkRunner.Run<BenchAsNoTracking>(); // database provider could matter (sqlite/mssql/postgres/etc)











