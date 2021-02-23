using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Text;

namespace _09._Increase_Age_Stored_Procedure
{
    class Program
    {
        private const string connectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";

        static void Main(string[] args)
        {
            string id = Console.ReadLine();

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            IncreaseMinionAge(id, connection);

            string newMinionAgeAndName = GetMinionsAgeAndName(id, connection);

            Console.WriteLine(newMinionAgeAndName);
        }

        private static string GetMinionsAgeAndName(string minionId, SqlConnection sqlConnection)
        {
            StringBuilder sb = new StringBuilder();

            string getNameAndAge =  @"SELECT Name, Age 
                                     FROM Minions 
                                     WHERE Id = @Id";
            using SqlCommand getNameAndAgeCmd = new SqlCommand(getNameAndAge, sqlConnection);
            getNameAndAgeCmd.Parameters.AddWithValue("@id", minionId);

            using SqlDataReader dataReader = getNameAndAgeCmd.ExecuteReader();
            dataReader.Read();

            sb.AppendLine($"{dataReader["Name"]} - {dataReader["Age"]} years old");

            return sb.ToString().TrimEnd();
        }

        private static void IncreaseMinionAge(string minionId, SqlConnection sqlConnection)
        {
            string procedureName = "usp_GetOlder";

            using SqlCommand increaseAgeCmd = new SqlCommand(procedureName, sqlConnection);
            increaseAgeCmd.CommandType = CommandType.StoredProcedure;
            increaseAgeCmd.Parameters.AddWithValue("@id", minionId);

            increaseAgeCmd.ExecuteNonQuery();
        }
    }
}
