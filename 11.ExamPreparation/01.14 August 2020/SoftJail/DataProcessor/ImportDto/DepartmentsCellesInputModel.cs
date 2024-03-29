﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class DepartmentsCellesInputModel
    {
        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Name { get; set; }
        public CelleInputModel[] Cells { get; set; }
    }
    public class CelleInputModel
    {
        [Required]
        [Range(1, 1000)]
        public int CellNumber { get; set; }
        [Required]
        public bool HasWindow { get; set; }
    }
}
