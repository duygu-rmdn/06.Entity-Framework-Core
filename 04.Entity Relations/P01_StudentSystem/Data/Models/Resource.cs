using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "varchar(225)")]
        public string Url { get; set; }
        [Required]
        public ResourceType ResourceType { get; set; }
        [Required]
        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        
    }
}
