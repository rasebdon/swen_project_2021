namespace MTCG.Models
{
    public class User : DataObject
    {
        /// <summary>
        /// The unique username of the user
        /// </summary>
        public string Username { get; set; }

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
        public int Wins { get; set; }

        public bool IsAdmin { get; set; }

        public string? Bio { get; set; }
        public string? Image { get; set; }

        /// <summary>
        /// Constructor for using in normal environment
        /// </summary>
        /// <param name="username"></param>
        /// <param name="coins"></param>
        /// <param name="elo"></param>
        /// <param name="playedGames"></param>
        public User(Guid id, string username, string hash, int coins, ushort elo, int playedGames, string? bio, string? image, int wins) : base(id)
        {
            Username = username;
            Hash = hash;
            Coins = coins;
            ELO = elo;
            PlayedGames = playedGames;
            IsAdmin = false;
            Bio = bio;
            Image = image;
            Wins = wins;
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
                   Wins == user.Wins &&
                   IsAdmin == user.IsAdmin &&
                   Bio == user.Bio &&
                   Image == user.Image;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(ID);
            hash.Add(Username);
            hash.Add(Hash);
            hash.Add(Coins);
            hash.Add(ELO);
            hash.Add(PlayedGames);
            hash.Add(Wins);
            hash.Add(IsAdmin);
            hash.Add(Bio);
            hash.Add(Image);
            return hash.ToHashCode();
        }
    }
}
