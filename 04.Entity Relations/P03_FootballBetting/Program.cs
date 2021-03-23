using P03_FootballBetting.Data.Models;
using System;

namespace P03_FootballBetting
{
    class Program
    {
        static void Main(string[] args)
        {
            FootballBettingContext context = new FootballBettingContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
