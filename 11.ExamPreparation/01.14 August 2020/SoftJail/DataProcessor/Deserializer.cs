namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var dtoResulr =
                JsonConvert.DeserializeObject<IEnumerable<DepartmentsCellesInputModel>>(jsonString);
            foreach (var departmentCell in dtoResulr)
            {
                if (!IsValid(departmentCell) || departmentCell.Cells.Length == 0 || departmentCell.Cells.Any(c => !IsValid(c)))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var depCell = new Department
                {
                    Name = departmentCell.Name,
                    Cells = departmentCell.Cells.Select(c => new Cell
                    {
                        CellNumber = c.CellNumber,
                        HasWindow = c.HasWindow
                    }).ToList()
                };
                sb.AppendLine($"Imported {depCell.Name} with {depCell.Cells.Count} cells");
                context.Departments.Add(depCell);
            }
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var dtoPrisons = JsonConvert.DeserializeObject<IEnumerable<PrisonerMailInputModel>>(jsonString);
            var sb = new StringBuilder();
            
            foreach (var prisonerDto in dtoPrisons)
            {

                if (!IsValid(prisonerDto) || !IsValid(prisonerDto.Mails))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var isValidRel = DateTime.TryParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime relDate);

                var isValidIncr = DateTime.ParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                var prisoner = new Prisoner
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = isValidIncr,
                    ReleaseDate = isValidRel ? (DateTime?)relDate : null,
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId,
                    Mails = prisonerDto.Mails.Select(m => new Mail
                    {
                        Description = m.Description,
                        Sender = m.Sender,
                        Address = m.Address
                    }).ToArray()
                };
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
                context.Prisoners.Add(prisoner);

            }
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validOfficers = new List<Officer>();

            var officerPrisoners = XmlConverter.Deserializer<OfficersPrisonersInputModel>(xmlString, "Officers");

            foreach (var officerPrisoner in officerPrisoners)
            {
                if (!IsValid(officerPrisoner))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var officer = new Officer
                {
                    FullName = officerPrisoner.Name,
                    Salary = officerPrisoner.Money,
                    DepartmentId = officerPrisoner.DepartmentId,
                    Position = Enum.Parse<Position>(officerPrisoner.Position),
                    Weapon = Enum.Parse<Weapon>(officerPrisoner.Weapon)
                };
                foreach (var prisoner in officerPrisoner.Prisoners)
                {
                    officer.OfficerPrisoners.Add(new OfficerPrisoner
                    {
                        PrisonerId = prisoner.Id,
                    });
                }

                validOfficers.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");

            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}