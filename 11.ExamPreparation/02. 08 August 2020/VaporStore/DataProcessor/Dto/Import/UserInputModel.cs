using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class UserInputModel
    {
        [Required]
        [RegularExpression("^[A-Z][a-z]{2,} [A-Z][a-z]{2,}$")]
        public string FullName { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        
        [Range(3, 103)]
        public int Age { get; set; }
        public IEnumerable<CardsInputModel> Cards { get; set; }
    }
    public class CardsInputModel
    {
        [Required]
        [RegularExpression("^[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}$")]
        public string Number { get; set; }
        [Required]
        [RegularExpression("^[0-9]{3}$")]
        public string CVC { get; set; }
        [Required]
        public CardType? Type { get; set; }
    }
}
/*	Number – text, which consists of 4 pairs of 4 digits, separated by spaces (ex. “1234 5678 9012 3456”) (required)
•	Cvc – text, which consists of 3 digits (ex. “123”) (required)
•	Type – enumeration of type CardType, with possible values (“Debit”, “Credit”) (required)
*/
