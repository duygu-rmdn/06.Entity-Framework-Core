using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace BookShop.DataProcessor.ImportDto
{
    [XmlType("Book")]
    public class BooksInputModel
    {
        [XmlElement("Name")]
        [MinLength(3)]
        [MaxLength(30)]
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 3)]
        [XmlElement("Genre")]
        public string Genre { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        [XmlElement("Price")]
        public decimal Price { get; set; }

        [XmlElement("Pages")]
        [Range(50, 5000)]
        public int Pages { get; set; }
        [Required]
        [XmlElement("PublishedOn")]
        public string PublishedOn { get; set; }
    }
}
/*[Required]
        [MinLength(3)]
        [MaxLength(30)]
        [XmlElement("Name")]
        
        public string Name { get; set; }

        [Range(1,3)]
        [XmlElement("Genre")]
        public int Genre { get; set; }

        [XmlElement("Price")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [XmlElement("Pages")]
        [Range(50, 5000)]
        public int Pages { get; set; }

        [Required]
        [XmlElement("PublishedOn")]
        public string PublishedOn { get; set; }
*/