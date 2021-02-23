using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _07._Print_All_Minion_Names
{
    class Program
    {
        private const string connectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";

        static void Main(string[] args)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string getMinionNames = @"SELECT [Name] FROM Minions";
            SqlCommand command = new SqlCommand(getMinionNames, connection);
            SqlDataReader reader = command.ExecuteReader();

            List<string> names = new List<string>();

            while (reader.Read())
            {
                names.Add(reader["Name"].ToString());
            }

            for (int i = 0; i < names.Count / 2; i++)
            {
                Console.WriteLine(names[i]);
                Console.WriteLine(names[names.Count - 1 - i]);
            }

            if (names.Count % 2 == 1)
            {
                Console.WriteLine(names[names.Count / 2]);
            }

        }
    }
}
