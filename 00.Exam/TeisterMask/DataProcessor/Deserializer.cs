namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using TeisterMask.DataProcessor.ImportDto;
    using System.ComponentModel.DataAnnotations;

    using Data;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;
    using System.Linq;
    using System.Text;
    using TeisterMask.Data.Models;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var dto = XmlConverter.Deserializer<ProjectsInputModel>(xmlString, "Projects");

            foreach (var proj in dto)
            {
                if (!IsValid(proj))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime openDate;
                var isOpDAte = DateTime.TryParseExact(proj.OpenDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out openDate);

                if (isOpDAte == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var result = new Project
                {
                    Name = proj.Name,
                    OpenDate = openDate
                };

                if (proj.DueDate != null)
                {
                    DateTime dueDate;
                    bool isDue = DateTime.TryParseExact(proj.DueDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate);

                    if (isDue)
                    {
                        result.DueDate = dueDate;
                    }
                }

                foreach (var tDto in proj.Tasks)
                {
                    if (!IsValid(tDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime taskOpD;
                    bool isParsedOpenDate = DateTime.TryParseExact(tDto.OpenDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out taskOpD);

                    DateTime taskDueDate;
                    bool isParsedDueDate = DateTime.TryParseExact(tDto.DueDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out taskDueDate);

                    if (isParsedOpenDate = false || isParsedDueDate == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (result.DueDate != null)
                    {
                        if (taskDueDate > result.DueDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                    }
                    if (taskOpD < result.OpenDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var resT = new Task
                    {
                        Name = tDto.Name,
                        DueDate = taskDueDate,
                        OpenDate = taskOpD,
                        ExecutionType = Enum.Parse<ExecutionType>(tDto.ExecutionType.ToString()),
                        LabelType = Enum.Parse<LabelType>(tDto.LabelType.ToString())
                    };

                    result.Tasks.Add(resT);
                }

                context.Projects.Add(result);
                sb.AppendLine($"Successfully imported project - {result.Name} with {result.Tasks.Count} tasks.");
                context.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var dto = JsonConvert.DeserializeObject<EmployeeInputModel[]>(jsonString);

            var sb = new StringBuilder();

            foreach (var employeeDto in dto)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var result = new Employee
                {
                    Username = employeeDto.Username,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone
                };

                foreach (var task in employeeDto.Tasks.Distinct())
                {
                    if (!(context.Tasks.Any(t => t.Id == task)))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var empTask = new EmployeeTask
                    {
                        TaskId = task
                    };

                    result.EmployeesTasks.Add(empTask);
                }

                context.Employees.Add(result);
                sb.AppendLine($"Successfully imported employee - {result.Username} with {result.EmployeesTasks.Count()} tasks.");
                context.SaveChanges();
            }

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}