using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.InputModels
{
    [XmlType("Sale")]
    public class SalesInputModel
    {
        [XmlElement("carId")]
        public int CarId { get; set; }
        [XmlElement("customerId")]
        public int CustomerId { get; set; }
        [XmlElement("discount")]
        public int Discount { get; set; }
    }
}
