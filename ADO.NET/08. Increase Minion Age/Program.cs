using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace _08._Increase_Minion_Age
{
    class Program
    {
        private const string connectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";

        static void Main(string[] args)
        {
            int[] idsToGrowUpAge = Console.ReadLine()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            foreach (var id in idsToGrowUpAge)
            {
                string updateString = @"UPDATE Minions
                                       SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                     WHERE Id = @Id";

                SqlCommand updateCommand = new SqlCommand(updateString, connection);
                updateCommand.Parameters.AddWithValue("@Id", id);
                updateCommand.ExecuteNonQuery();
            }

            string getNames = "SELECT [Name], Age FROM Minions";
            SqlCommand getNamesCmd = new SqlCommand(getNames, connection);
            using SqlDataReader reader = getNamesCmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
            }
        }
    }
}
