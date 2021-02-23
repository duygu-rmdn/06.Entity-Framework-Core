using Microsoft.Data.SqlClient;
using System;

namespace _06._Remove_Villain
{
    class Program
    {
        private const string connectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";

        static void Main(string[] args)
        {
            int villainToRemove = int.Parse(Console.ReadLine());

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction();

            string getVillainName = @"SELECT [Name] FROM Villains WHERE Id = @villainId";
            SqlCommand getNameCmd = new SqlCommand(getVillainName, connection, transaction);
            getNameCmd.Parameters.AddWithValue("@villainId", villainToRemove);
            string villainName = getNameCmd.ExecuteScalar()?.ToString();

            if (villainName is null)
            {
                Console.WriteLine("No such villain was found.");
                return;
            }

            string deleteMinionsVillains = @"DELETE FROM MinionsVillains WHERE VillainId = @villainId";
            SqlCommand deletMinVilCmd = new SqlCommand(deleteMinionsVillains, connection, transaction);
            deletMinVilCmd.Parameters.AddWithValue("@villainId", villainToRemove);

            int minionsCount = deletMinVilCmd.ExecuteNonQuery();

            string deleteVillains = @"DELETE FROM Villains WHERE Id = @villainId";
            SqlCommand deleteVillainCmd = new SqlCommand(deleteVillains, connection, transaction);
            deleteVillainCmd.Parameters.AddWithValue("@villainId", villainToRemove);
            deleteVillainCmd.ExecuteNonQuery();

            Console.WriteLine($"{villainName} was deleted.");
            Console.WriteLine($"{minionsCount} minions were released.");

            transaction.Commit();
        }
    }
}
