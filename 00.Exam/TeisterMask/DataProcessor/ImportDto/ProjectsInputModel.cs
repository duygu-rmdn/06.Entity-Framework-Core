using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Project")]
    public class ProjectsInputModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required]
        public string OpenDate { get; set; }


        public string DueDate { get; set; }

        [XmlArray("Tasks")]
        public TaskInputModel[] Tasks { get; set; }
    }



    [XmlType("Task")]
    public class TaskInputModel
    {

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required]
        public string OpenDate { get; set; }

        [Required]
        public string DueDate { get; set; }

        public int ExecutionType { get; set; }

        public int LabelType { get; set; }

    }
}
/*Name - text with length [2, 40] (required)
•	OpenDate - date and time (required)
•	DueDate - date and time (required)
•	ExecutionType - enumeration of type ExecutionType, with possible values (ProductBacklog, SprintBacklog, InProgress, Finished) (required)
•	LabelType - enumeration of type LabelType, with possible values (Priority, CSharpAdvanced, JavaAdvanced, EntityFramework, Hibernate) (required)
*/
