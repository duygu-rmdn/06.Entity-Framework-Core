﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Town
    {
        public Town()
        {
            Teams = new HashSet<Team>();
        }
        public int TownId { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        public ICollection<Team> Teams { get; set; }
    }
}
