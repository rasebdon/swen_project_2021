namespace MTCG.DAL
{
    public class DatabaseConfiguration
    {
        public static DatabaseConfiguration DefaultConfiguration
        {
            get
            {
                return new DatabaseConfiguration("localhost", "production", "production", "1234");
            }
        }
        public string ConnectionString
        {
            get
            {
                return $"Host={IP};Port={Port};Database={Database};Username={Username};Password={Password};IncludeErrorDetail=true;";
            }
        }
        public string IP { get; }
        public ushort Port { get; }
        public string Database { get; }
        public string Username { get; }
        public string Password { get; }

        public DatabaseConfiguration(string ip, string database, string username, string password, ushort port = 5432)
        {
            IP = ip;
            Port = port;
            Database = database;
            Username = username;
            Password = password;
        }
    }
}
