namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlSerializer = new XmlSerializer(typeof(ProjectXmlDto[]), new XmlRootAttribute("Projects"));

            var proj = context.Projects
                .ToArray()
                .Where(x => x.Tasks.Count > 0)
                .Select(p => new ProjectXmlDto()
                {
                    HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                    ProjectName = p.Name,
                    TasksCount = p.Tasks.Count,
                    Tasks = p.Tasks.ToArray().Select(t => new TaskXmlDto()
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    }) .OrderBy(x => x.Name)
                        .ToArray()
                })
                .OrderByDescending(x => x.TasksCount)
                .ThenBy(x => x.ProjectName)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            xmlSerializer.Serialize(stringWriter, proj, namespaces);
            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var result = context.Employees
                .Where(x => x.EmployeesTasks.Any(t => t.Task.OpenDate >=date))
                .ToArray()
                .Select(x => new
                {
                    Username = x.Username,
                    Tasks = x.EmployeesTasks
                    .ToArray()
                    .Where(x => x.Task.OpenDate >= date)
                    .OrderByDescending(x => x.Task.DueDate)
                    .ThenBy(x => x.Task.Name)
                    .Select(a => new
                    {
                        TaskName = a.Task.Name,
                        OpenDate = a.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = a.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = a.Task.LabelType.ToString(),
                        ExecutionType = a.Task.ExecutionType.ToString()
                    })
                    .ToArray()
                })
                .OrderByDescending(x => x.Tasks.Length)
                .ThenBy(x => x.Username)
                .Take(10)
                .ToArray();

            var res = JsonConvert.SerializeObject(result, Formatting.Indented);

            return res;
        }
    }
}