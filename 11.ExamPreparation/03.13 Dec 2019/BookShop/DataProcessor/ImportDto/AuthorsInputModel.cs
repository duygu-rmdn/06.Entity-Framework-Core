using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{
    public class AuthorsInputModel
    {
        [MaxLength(20)]
        [MinLength(3)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(20)]
        [MinLength(3)]
        [Required]
        public string LastName { get; set; }
        [Required]
        [RegularExpression("^[0-9]{3}-[0-9]{3}-[0-9]{4}$")]
        public string Phone { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public BooksModel[] Books { get; set; }
    }
    public class BooksModel
    {
        public int? Id { get; set; }
    }
}
