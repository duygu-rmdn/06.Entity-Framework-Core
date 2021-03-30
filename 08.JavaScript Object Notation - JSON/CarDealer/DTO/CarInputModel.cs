using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarDealer.DTO
{
    public class CarInputModel
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int TravelledDistance { get; set; }
        public IEnumerable<int> PartsId { get; set; } = new List<int>();
    }
}
