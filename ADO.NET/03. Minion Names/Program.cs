using Microsoft.Data.SqlClient;
using System;
using System.Text;

namespace _03._Minion_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            string connection = @"Server=.;Database=MinionsDB;Integrated Security=true;";
            using SqlConnection sqlConnection = new SqlConnection(connection);
            sqlConnection.Open();

            int villainId = int.Parse(Console.ReadLine());
            string getVillain = "SELECT [Name] FROM Villains WHERE Id = @villainId";
            SqlCommand getVillainCmd = new SqlCommand(getVillain, sqlConnection);
            getVillainCmd.Parameters.AddWithValue("@villainId", villainId);

            string villainName = getVillainCmd.ExecuteScalar() as string;

            if (villainName  is null)
            {
                Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                return;
            }

            string getMinions = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

            SqlCommand getMinionsCmd = new SqlCommand(getMinions, sqlConnection);

            getMinionsCmd.Parameters.AddWithValue("@Id", villainId);
            using SqlDataReader reader = getMinionsCmd.ExecuteReader();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Villain: {villainName}");

            bool NoMinions = true;
            int counter = 0;
            while (reader.Read())
            {
                counter++;
                NoMinions = false;
                string minionName = (string)reader["Name"];
                int age = (int)reader["Age"];

                sb.AppendLine($"{counter}. {minionName} {age}");
            }
            if (NoMinions)
            {
                sb.AppendLine("(no minions)");
            }
            Console.WriteLine(sb.ToString().TrimEnd());
        }
    }
}
