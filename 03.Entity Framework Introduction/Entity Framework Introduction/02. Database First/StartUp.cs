using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            //string getEmployeesFullInformation = GetEmployeesFullInformation(context);
            //string getEmployeesWithSalaryOver50000 = GetEmployeesWithSalaryOver50000(context);
            //string getEmployeesFromResearchAndDevelopment = GetEmployeesFromResearchAndDevelopment(context);
            //string addNewAddressToEmployee = AddNewAddressToEmployee(context);
            //string getEmployeesInPeriod = GetEmployeesInPeriod(context);
            //string getAddressesByTown = GetAddressesByTown(context);
            string getEmployee147 = GetEmployee147(context);
            //string getDepartmentsWithMoreThan5Employees = GetDepartmentsWithMoreThan5Employees(context);
            //string getLatestProjects = GetLatestProjects(context);
            //string increaseSalaries = IncreaseSalaries(context);
            //string getEmployeesByFirstNameStartingWithSa = GetEmployeesByFirstNameStartingWithSa(context);
            //string deleteProjectById = DeleteProjectById(context);
            //string removeTown = RemoveTown(context);
            Console.WriteLine(removeTown);
        }
        //P15:
        public static string RemoveTown(SoftUniContext context)
        {
            int townId = context.Towns
                .Where(t => t.Name == "Seattle")
                .Select(t => t.TownId)
                .FirstOrDefault();

            var addresses = context.Addresses
                .Where(a => a.TownId == townId)
                .ToList();


            foreach (var emp in context.Employees)
            {
                if (addresses.Contains(emp.Address))
                {
                    emp.AddressId = null;
                }
            }

            context.Addresses.RemoveRange(addresses);
            context.Towns.Remove(context.Towns.FirstOrDefault(t => t.TownId == townId));

            context.SaveChanges();

            string result = addresses.Count == 1 ? $"{addresses.Count} address in Seattle was deleted"
                : $"{addresses.Count} addresses in Seattle were deleted";

            return result;
        }

        //P14:
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var project = context.Projects.Find(2);
            var employeeProject = context.EmployeesProjects.Where(x => x.ProjectId == 2);

            foreach (var item in employeeProject)
            {
                context.EmployeesProjects.Remove(item);
            }

            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects.Take(10);
            foreach (var proj in projects)
            {
                sb.AppendLine(proj.Name);
            }

            return sb.ToString().Trim();
        }

        //P13:
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Where(e => e.FirstName.ToLower().StartsWith("sa"))
                .Select(e => new 
                { 
                    e.FirstName,
                    e.LastName,
                    e.Salary,
                    e.JobTitle
                }).OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle} - (${emp.Salary:f2})");
            }
            return sb.ToString().TrimEnd();
        }

        //P12:
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design" || e.Department.Name == "Marketing" || e.Department.Name == "Information Services");
            foreach (var emp in employees)
            {
                emp.Salary *= 1.12M;
            }
            context.SaveChanges();
             var increased = employees.Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var emp in increased)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} (${emp.Salary:f2})");
            }
            return sb.ToString().TrimEnd();
        }

        //P11:
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                
                .Take(10)
                .Select(p => new 
                { 
                    p.StartDate,
                    p.Description,
                    p.Name
                })
                .OrderBy(p =>p.Name)
                .ToList();

            foreach (var project in projects)
            {
                string startDate = project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                sb.AppendLine(project.Name);
                sb.AppendLine(project.Description);
                sb.AppendLine(startDate);
            }
            return sb.ToString().TrimEnd();
        }

        //P10:
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepName = d.Name,
                    ManagerName = $"{d.Manager.FirstName} {d.Manager.LastName}",
                    Employees = d.Employees
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    })
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToList()
                })
                .ToList();

            foreach (var dep in departments)
            {
                sb.AppendLine($"{dep.DepName} - {dep.ManagerName}");

                foreach (var employee in dep.Employees)
                {
                    sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //P09:
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employee147 = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Procects = e.EmployeesProjects
                    .Select(ep => ep.Project.Name)
                    .OrderBy(p => p)
                    .ToList()
                }).FirstOrDefault();

            sb.AppendLine($"{employee147.FirstName} {employee147.LastName} - {employee147.JobTitle}");

            foreach (var project in employee147.Procects)
            {
                sb.AppendLine(project);
            }

            return sb.ToString().TrimEnd();
        }

        //P08:
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var addresses = context.Addresses
                .Select(a => new
                {
                    a.AddressText,
                    townName = a.Town.Name,
                    empCount = a.Employees.Count
                })
                .OrderByDescending(a => a.empCount)
                .ThenBy(a => a.townName)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .ToList();
            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.townName} - {address.empCount} employees");
            }
            return sb.ToString().TrimEnd();
        }

        //P07:
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var empployees = context
                .Employees
                .Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001
                                                    && ep.Project.StartDate.Year <= 2003))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    menegFir = e.Manager.FirstName,
                    menegLast = e.Manager.LastName,
                    Projects = e.EmployeesProjects
                        .Select(ep => new
                        {
                            ep.Project.Name,
                            ep.Project.StartDate,
                            ep.Project.EndDate
                        }).ToList()
                }).Take(10)
                .ToList();

            foreach (var emp in empployees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - Manager: {emp.menegFir} {emp.menegLast}");
                foreach (var project in emp.Projects)
                {
                    string startDate = project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    string endDate = project.EndDate == null? "not finished" :
                        project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    sb.AppendLine($"--{project.Name} - {startDate} - {endDate}");
                }
            }
            return sb.ToString().TrimEnd();
        }

        //P06:
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            context.Addresses.Add(address);

            Employee employee = context.Employees
                .Where(e => e.LastName == "Nakov")
                .FirstOrDefault();

            employee.Address = address;
            context.SaveChanges();

            var addresses = context.Employees
                .OrderByDescending(a => a.AddressId)
                .Take(10)
                .Select(a => a.Address.AddressText)
                .ToList();

            foreach (var addr in addresses)
            {
                sb.AppendLine(addr.ToString());
            }
            return sb.ToString().TrimEnd();
        }

        //P05:
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} from {emp.DepName} - ${emp.Salary:f2}");
            }

            return sb.ToString();
        }


        //P04:
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context
                .Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary,
                    e.Department
                })
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }


        //P03: 
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.EmployeeId)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
