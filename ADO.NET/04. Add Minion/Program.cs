using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace _04._Add_Minion
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";

            string[] minionInput = Console.ReadLine().Split(": ", StringSplitOptions.RemoveEmptyEntries).ToArray();
            string[] minionInfo = minionInput[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();

            string minionName = minionInfo[0];
            string minionAge = minionInfo[1];
            string minionTownInfo = minionInfo[2];

            string[] villaiInput = Console.ReadLine().Split(": ", StringSplitOptions.RemoveEmptyEntries).ToArray();
            string villainName = villaiInput[1];

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string villainId = GetVillainId(villainName, connection);
            if (villainId is null)
            {
                AddVillainIntoDB(villaiInput[1], connection);
                villainId = GetVillainId(villainName, connection);
                Console.WriteLine($"Villain {villainName} was added to the database.");
            }

            string TownId = IsMinionTownExist(minionTownInfo, connection);
            if (TownId is null)
            {
                AddMinionTown(minionTownInfo, connection);
                TownId = IsMinionTownExist(minionTownInfo, connection);

                Console.WriteLine($"Town {minionTownInfo} was added to the database.");
            }

            AddMinionInDB(minionName, minionAge, TownId, connection);

            string minionID = GetMinionId(minionName, connection);

            SetMinionToVillain(minionID, villainId, connection);

            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
        }

        private static void SetMinionToVillain(string minionId, string villainId, SqlConnection sqlConnection)
        {
            string servantQuery =
                @"INSERT INTO MinionsVillains (MinionId, VillainId)         
                    VALUES (@villainId, @minionId)";
            SqlCommand servantCommand = new SqlCommand(servantQuery, sqlConnection);
            servantCommand.Parameters.AddRange(new[]
            {
                new SqlParameter("@villainId", villainId),
                new SqlParameter("@minionId", minionId)
            });

            servantCommand.ExecuteNonQuery();
        }

        private static string GetMinionId(string minionName, SqlConnection sqlConnection)
        {
            string minionIdQuery = @"SELECT Id FROM Minions WHERE Name = @Name";
            SqlCommand minionIdCmd = new SqlCommand(minionIdQuery, sqlConnection);
            minionIdCmd.Parameters.AddWithValue("@Name", minionName);

            string minionId = minionIdCmd.ExecuteScalar()?.ToString();

            return minionId;
        }

        private static string GetVillainId(string villanName, SqlConnection sqlConnection)
        {
            string villainIdQuery = @"SELECT Id FROM Villains WHERE Name = @Name";
            SqlCommand villainIdCmd = new SqlCommand(villainIdQuery, sqlConnection);
            villainIdCmd.Parameters.AddWithValue("@Name", villanName);

            string villainId = villainIdCmd.ExecuteScalar()?.ToString();

            return villainId;
        }

        private static void AddVillainIntoDB(string villainName, SqlConnection sqlConnection)
        {
            string addVillainQuery =
                @"INSERT INTO Villains (Name, EvilnessFactorId)  
                    VALUES (@villainName, 4)";
            using SqlCommand addVillain = new SqlCommand(addVillainQuery, sqlConnection);
            addVillain.Parameters.AddWithValue("@villainName", villainName);

            addVillain.ExecuteNonQuery();
        }

        static void AddMinionInDB(string minionName, string minionAge, string minionTownInfo, SqlConnection sqlConnection)
        {
            string addMinionQuery =
                @"INSERT INTO Minions (Name, Age, TownId) 
                    VALUES (@name, @age, @townId)";

            using SqlCommand addNewMinion = new SqlCommand(addMinionQuery, sqlConnection);
            addNewMinion.Parameters.AddRange(new[]
            {
                new SqlParameter("@name", minionName),
                new SqlParameter("@age", minionAge),
                new SqlParameter("@townId", minionTownInfo)
            });

            addNewMinion.ExecuteNonQuery();
        }

        private static string IsMinionTownExist(string minionTown, SqlConnection sqlConnection)
        {
            string sqlVillainCommandQuery = @"SELECT Id FROM Towns WHERE Name = @townName";
            using SqlCommand MinionTownCommand = new SqlCommand(sqlVillainCommandQuery, sqlConnection);
            MinionTownCommand.Parameters.AddWithValue("@townName", minionTown);

            string isMinionTownExist = MinionTownCommand.ExecuteScalar()?.ToString();

            return isMinionTownExist;
        }

        private static void AddMinionTown(string minionTown, SqlConnection sqlConnection)
        {
            string addTownQuery =
                @"INSERT INTO Towns (Name) 
                    VALUES (@townName)";

            using SqlCommand addMinionTownCmd = new SqlCommand(addTownQuery, sqlConnection);
            addMinionTownCmd.Parameters.AddWithValue("@townName", minionTown);

            addMinionTownCmd.ExecuteNonQuery();
        }
    }
}
