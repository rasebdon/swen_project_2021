using System.Collections.Generic;
using System.Collections.Specialized;

namespace MTCG.Models
{
    class User : DataObject
    {
        /// <summary>
        /// The unique username of the user
        /// </summary>
        public string Username { get; }
        /// <summary>
        /// The amount of coins the user has (display only)
        /// </summary>
        public int Coins { get; set; }

        public string SessionToken { get; set; }

        public User(OrderedDictionary row) : base(row)
        {
            Username = row["username"].ToString();
            Coins = (int)row["coins"];
        }
        public User(uint id, string username, int coins) : base(id)
        {
            Username = username;
            Coins = coins;
        }
    }
}
