namespace MTCG.Models
{
    public class User : DataObject
    {
        /// <summary>
        /// The unique username of the user
        /// </summary>
        public string Username { get; }
        public string Hash { get; set; }
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
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Constructor for using in normal environment
        /// </summary>
        /// <param name="username"></param>
        /// <param name="coins"></param>
        /// <param name="elo"></param>
        /// <param name="playedGames"></param>
        public User(Guid id, string username, string hash, int coins, ushort elo, int playedGames) : base(id)
        {
            Username = username;
            Hash = hash;
            Coins = coins;
            ELO = elo;
            PlayedGames = playedGames;
            IsAdmin = false;
            SessionToken = string.Empty;
        }

        public override bool Equals(object? obj)
        {
            return obj is User user &&
                   ID.Equals(user.ID) &&
                   Username == user.Username &&
                   Hash == user.Hash &&
                   Coins == user.Coins &&
                   ELO == user.ELO &&
                   PlayedGames == user.PlayedGames &&
                   SessionToken == user.SessionToken &&
                   IsAdmin == user.IsAdmin;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Username, Hash, Coins, ELO, PlayedGames, SessionToken, IsAdmin);
        }

    }
}
