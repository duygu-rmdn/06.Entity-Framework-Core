using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.OutputModels
{
    [XmlType("car")]
    public class CarPartsOutputModel
    {
        [XmlAttribute("make")]
        public string Make { get; set; }
        [XmlAttribute("model")]
        public string Model { get; set; }
        [XmlAttribute("travelled-distance")]
        public long TraveledDistance { get; set; }
        [XmlArray("parts")]
        public PartsOutputModel[] Parts { get; set; }
    }
}
