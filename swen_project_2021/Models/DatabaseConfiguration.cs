namespace MTCG.Models
{
    public class DatabaseConfiguration
    {
        public static DatabaseConfiguration DefaultConfiguration
        {
            get
            {
                return new DatabaseConfiguration("localhost", "mtcg", "mtcgadmin", "p1s2w3r4");
            }
        }
        public string ConnectionString
        {
            get
            {
                return $"Host={IP};Database={Database};Username={Username};Password={Password}";
            }
        }
        public string IP { get; }
        public string Database { get; }
        public string Username { get; }
        public string Password { get; }

        public DatabaseConfiguration(string ip, string database, string username, string password)
        {
            IP = ip;
            Database = database;
            Username = username;
            Password = password;
        }
    }
}
