using MTCG.DAL;
using System;
using System.Text.Json.Serialization;

namespace MTCG.Models
{
    [Serializable]
    [TableName("users")]
    public class User : DatabaseObject
    {
        /// <summary>
        /// The unique username of the user
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; }

        /// <summary>
        /// The amount of coins the user has (display only)
        /// </summary>
        [JsonPropertyName("coins")]
        public int Coins { get; set; }

        /// <summary>
        /// The ELO is representative for the users skill in battles
        /// </summary>
        [JsonPropertyName("elo")]
        public ushort ELO { get; set; }

        [JsonPropertyName("played_games")]
        public int PlayedGames { get; set; }

        [JsonPropertyName("session_token")]
        public string SessionToken { get; set; }

        [JsonIgnore]
        public bool IsAdmin { get; }

        public User(string username, int coins, ushort elo, int playedGames) : base(Guid.NewGuid())
        {
            Username = username;
            Coins = coins;
            ELO = elo;
            PlayedGames = playedGames;
            IsAdmin = false;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
