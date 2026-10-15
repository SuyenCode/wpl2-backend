namespace ClassLibTeam12.Data.Framework
{
    static class Settings
    {
        public static string GetConnectionString()
        {
            string connectionString = "Trusted_Connection=True;";

            connectionString = "User ID = PxlUser_12;";
            connectionString += "Password = YOUR_DB_PASSWORD;";
            connectionString += $@"Server = YOUR_SERVER;";
            connectionString += $"Database = Db2025Team_12";

            return connectionString;
        }
    }
}
