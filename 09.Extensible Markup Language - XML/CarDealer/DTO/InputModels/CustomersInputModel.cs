using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.InputModels
{
    [XmlType("Customer")]
    public class CustomersInputModel
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("birthDate")]
        public DateTime BirthDay { get; set; }
        [XmlElement("isYoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}
