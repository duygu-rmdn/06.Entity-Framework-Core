﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class PrisonerMailInputModel
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FullName { get; set; }
        [Required]
        [RegularExpression("The [A-z][a-z]+")]
        public string Nickname { get; set; }
        [Required]
        [Range(18, 65)]
        public int Age { get; set; }
        [Required]
        public string IncarcerationDate { get; set; }
        public string? ReleaseDate { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Bail { get; set; }
        public int CellId { get; set; }
        public List<MailInputModel> Mails { get; set; }
    }
    public class MailInputModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string Sender { get; set; }
        [RegularExpression(@"^([A-Za-z\s1-9]+str.)$")]
        public string Address { get; set; }
    }
}
/*
  
•	Description– text (required)
•	Sender – text (required)
•	Address – text, consisting only of letters, spaces and numbers, which ends with “ str.” (required) (Example: “62 Muir Hill str.“)
•	PrisonerId - integer, foreign key (required)
•	Prisoner – the mail's Prisoner (required)
*/
