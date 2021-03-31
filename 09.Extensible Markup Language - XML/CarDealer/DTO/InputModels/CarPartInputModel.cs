using System.Xml.Serialization;

namespace CarDealer.DTO.InputModels
{
    [XmlType("partId")]
    public class CarPartInputModel
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
