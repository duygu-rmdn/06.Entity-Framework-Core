using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.OutputModels
{
    [XmlType("customer")]
    public class CustomerOutputModel
    {
        [XmlAttribute("full-name")]
        public string Name { get; set; }

        [XmlAttribute("bought-cars")]
        public int BoughtCars { get; set; }

        [XmlAttribute("spent-money")]
        public decimal SpendMoney { get; set; }
    }
}
