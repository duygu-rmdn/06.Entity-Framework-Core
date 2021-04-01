using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.OutputModels
{
    [XmlType("sale")]
    public class SaleOutputModel
    {
        [XmlElement("car")]
        public CarSalesOutputModel Car { get; set; }
        [XmlElement("discount")]
        public decimal Discount { get; set; }
        [XmlElement("customer-name")]
        public string Name { get; set; }
        [XmlElement("price")]
        public decimal Price { get; set; }
        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }
    }
}
