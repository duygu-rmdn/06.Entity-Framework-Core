﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.OutputModels
{
    [XmlType("car")]
    public class CarsOutputModel
    {
        [XmlElement("make")]
        public string Make { get; set; }
        [XmlElement("model")]
        public string Model { get; set; }
        [XmlElement("travelled-distance")]
        public long TraveledDistance { get; set; }
    }
}
