using CryptSharp;
using MTCG.Controller.Exceptions;
using MTCG.Database;
using MTCG.Models;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace MTCG.Controller
{
    class UserController : Singleton<UserController>
    {
        public List<User> LoggedInUsers { get; }

        public UserController()
        {
            LoggedInUsers = new();
        }

        /// <summary>
        /// Registers a new user in the database
        /// </summary>
        /// <param name="username">The desired username</param>
        /// <param name="email">The desired email</param>
        /// <param name="password">The desired password</param>
        /// <returns>The registered user object</returns>
        public User Register(string username, string password)
        {
            /// Plain argument checks
            // Check username
            if (!Regex.IsMatch(username, "[a-zA-Z]{4,24}"))
                throw new ArgumentException("Username is not valid!");
            // Check password
            //if (!Regex.IsMatch(password, @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&\-+=()])(?=\S+$).{8,20}$"))
            //    throw new ArgumentException("Password is not valid!");

            /// Database checks
            // Check if username is available (check in lower case)
            var cmd = new NpgsqlCommand(
                $"SELECT username FROM users WHERE LOWER(username)=@username;");
            cmd.Parameters.AddWithValue("username", username.ToLower());

            if (Server.Instance.Database.SelectSingle(cmd) != null)
                throw new DuplicateEntryException(username);

            /// Create password hash and insert
            string hash = Crypter.Blowfish.Crypt(password);
            string sql = $"INSERT INTO users (username, hash) VALUES (@username, @hash);";
            cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("hash", hash);

            // Check if user was successfully created
            if (Server.Instance.Database.ExecuteNonQuery(cmd) != 1)
            {
                throw new DatabaseInsertException(sql);
            }

            // Return the inserted user object
            return UserController.GetUser(username);
        }

        /// <summary>
        /// Returns a user on success and throws an InvalidCredentialsException on failure
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <param name="password">The plaintext password of the user</param>
        /// <returns>A user if the login was successful and throws exceptions on failure</returns>
        public User Login(string username, string password)
        {
            // Check for valid arguments
            if (username.Length <= 0 || password.Length <= 0)
                throw new ArgumentException("Username or password input is not valid!");

            var user = GetUser(username);

            if (user == null)
                throw new InvalidCredentialsException(username);

            // Check if user is already logged in
            if (LoggedInUsers.Contains(user))
                throw new AlreadyLoggedInException(user);

            bool authenticated = Crypter.CheckPassword(password, GetUserHash(username));
            if (!authenticated)
                throw new InvalidCredentialsException(username);

            // Add user to logged in users
            LoggedInUsers.Add(user);

            // Generate auth token
            user.SessionToken = $"{user.Username}-mtcgToken";

            return user;
        }

        /// <summary>
        /// Returns the user with the specified username or null on failure
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <returns>User object on success or null on failure</returns>
        public static User GetUser(string username)
        {
            // Create safe query
            var sql = "SELECT * FROM users WHERE username = @username";
            // Prepare statement
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("username", username);

            // Communicate with database
            OrderedDictionary row = Server.Instance.Database.SelectSingle(cmd);

            // Check if user exists
            if (row == null)
                return null;
            var user = new User(row);

            return user;
        }

        /// <summary>
        /// Returns the user with the specified ID or null on failure
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns>User object on success or null on failure</returns>
        public static User GetUser(int id)
        {
            // Create safe query
            var sql = "SELECT * FROM users WHERE id = @id";
            // Prepare statement
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", id);

            // Communicate with database
            OrderedDictionary row = Server.Instance.Database.SelectSingle(cmd);

            // Check if user exists
            if (row == null)
                return null;
            var user = new User(row);

            return user;
        }

        /// <summary>
        /// Gets the hash of a user with the given username
        /// </summary>
        /// <param name="username">The unique username of the user</param>
        /// <returns>The hash of the user or an empty string if there was
        /// no user with this username found</returns>
        public string GetUserHash(string username)
        {
            // Prepare sql
            var sql = "SELECT hash FROM users WHERE username=@username";
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("username", username);

            // Execute and process result
            return ParseHash(Server.Instance.Database.SelectSingle(cmd));
        }

        public string GetUserHash(int id)
        {
            // Prepare sql
            var sql = "SELECT hash FROM users WHERE id=@id";
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", id);

            // Execute and process result
            return ParseHash(Server.Instance.Database.SelectSingle(cmd));
        }

        /// <summary>
        /// Parses the user hash from the given row
        /// </summary>
        /// <param name="row">The row with the hash</param>
        /// <returns>The hash string if it was found in the row
        /// and an empty string if it was not</returns>
        private string ParseHash(OrderedDictionary row)
        {
            if (row == null) return "";
            return row["hash"].ToString();
        }


        /// <summary>
        /// Gets the card stack of the given user
        /// </summary>
        /// <param name="user">The user of which the card stack should be retrieved</param>
        /// <returns>The card stack of the given user</returns>
        public Dictionary<Card, int> GetUserCardStack(User user)
        {
            throw new NotImplementedException();

            var stack = new Dictionary<Card, int>
            {
                { new SpellCard(1, "WaterGoblin", 23), 5 }
            };
            return stack;
        }

        /// <summary>
        /// Buy command for the user for buying a card package
        /// </summary>
        /// <param name="user">The user who buys the package</param>
        public static void BuyPackage(User user)
        {
            const int cost = 5;
            const int cards = 5;

            throw new NotImplementedException();
        }
    }
}
