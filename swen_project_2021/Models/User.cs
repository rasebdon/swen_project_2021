using System.Collections.Generic;
using System.Collections.Specialized;

namespace MTCG.Models
{
    class User : IValueObject
    {
        /// <summary>
        /// The unique id of the user
        /// </summary>
        public uint ID { get; }
        /// <summary>
        /// The unique username of the user
        /// </summary>
        public string Username { get; }
        /// <summary>
        /// The amount of coins the user has (display only)
        /// </summary>
        public int Coins { get; set; }

        public User(uint id, string username)
        {
            ID = id;
            Username = username;
        }

        public static User Parse(OrderedDictionary row)
        {
            return new User(
                (uint)(int)row["ID"],
                row["username"].ToString());
        }
    }
}
