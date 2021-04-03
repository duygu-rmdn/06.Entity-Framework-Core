namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.Linq;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisonners = context.Prisoners
                .Where(x => ids.Contains(x.Id))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.FullName,
                    CellNumber = x.Cell.CellNumber,
                    Officers = x.PrisonerOfficers.Select(y => new
                    {
                        OfficerName = y.Officer.FullName,
                        Department = y.Officer.Department.Name
                    }).OrderBy(z => z.OfficerName)
                    .ToArray(),
                    TotalOfficerSalary = double.Parse(x.PrisonerOfficers.Select(z => z.Officer.Salary).Sum().ToString("F2"))
                }).OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            var result = JsonConvert.SerializeObject(prisonners, Formatting.Indented);
            return result;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var prisoners = context.Prisoners
                .Where(x => names.Contains(x.FullName))
                .Select(x => new PrisonerMailOutputModel
                {
                    Id = x.Id,
                    Name = x.FullName,
                    IncarcerationDate = x.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EncryptedMessages = x.Mails.Select(m => new EncrMess
                    {
                        Description = string.Join("", m.Description.Reverse())
                    }).ToArray()
                }).OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            var result = XmlConverter.Serialize(prisoners, "Prisoners");

            return result;
        }
    }
}