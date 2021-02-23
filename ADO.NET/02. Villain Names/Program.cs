using Microsoft.Data.SqlClient;
using System;

namespace _02._Villain_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            string connection = @"Server=.;Database=MinionsDB;Integrated Security=true;";
            using SqlConnection sqlConnection = new SqlConnection(connection);
            sqlConnection.Open();

            string getVillainNames = @"  SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                    FROM Villains AS v 
                    JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                GROUP BY v.Id, v.Name 
                  HAVING COUNT(mv.VillainId) > 3 
                ORDER BY COUNT(mv.VillainId)";

            SqlCommand command = new SqlCommand(getVillainNames, sqlConnection);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string name = reader["Name"].ToString();
                int minionsCount = (int)reader["MinionsCount"];

                Console.WriteLine($"{name} - {minionsCount}");
            }
       
        }
    }
}
