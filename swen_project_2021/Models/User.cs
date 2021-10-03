using System.Collections.Specialized;

namespace MTCG.Models
{
    public class User : DataObject
    {
        /// <summary>
        /// The unique username of the user
        /// </summary>
        public string Username { get; }
        /// <summary>
        /// The amount of coins the user has (display only)
        /// </summary>
        public int Coins { get; set; }
        /// <summary>
        /// The ELO is representative for the users skill in battles
        /// </summary>
        public ushort ELO { get; set; }

        public int PlayedGames { get; set; }

        public string SessionToken { get; set; }
        public bool IsAdmin { get; }

        public User(OrderedDictionary row) : base(row)
        {
            Username = row["username"].ToString();
            Coins = (int)row["coins"];
            ELO = (ushort)(int)row["elo"];
            PlayedGames = (int)row["played_games"];
            IsAdmin = bool.Parse(row["admin"].ToString());
        }
        public User(string username, int coins, ushort elo, int playedGames) : base()
        {
            Username = username;
            Coins = coins;
            ELO = elo;
            PlayedGames = playedGames;
            IsAdmin = false;
        }
    }
}
