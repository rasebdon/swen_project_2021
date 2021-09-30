using CryptSharp;
using MTCG.Controller.Exceptions;
using MTCG.Models;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using MTCG.Http;

namespace MTCG.Controller
{
    public class UserController : Singleton<UserController>
    {
        public readonly List<User> LoggedInUsers;

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

            if (Database.Instance.SelectSingle(cmd) != null)
                throw new DuplicateEntryException(username);

            User user = new(username, 20, 1000);

            /// Create password hash and insert
            string hash = Crypter.Blowfish.Crypt(password);
            string sql = $"INSERT INTO users (id, username, hash, coins, elo, admin) VALUES (@id, @username, @hash, @coins, @elo, false);";
            cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", user.ID);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("hash", hash);
            cmd.Parameters.AddWithValue("coins", (int)user.Coins);
            cmd.Parameters.AddWithValue("elo", (int)user.ELO);

            // Check if user was successfully created
            if (Database.Instance.ExecuteNonQuery(cmd) != 1)
            {
                throw new DatabaseInsertException(sql);
            }

            // Return the inserted user object
            return user;
        }

        public bool DeleteUser(Guid id)
        {
            NpgsqlCommand cmd = new("DELETE FROM users WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", id);
            return Database.Instance.ExecuteNonQuery(cmd) == 1;
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

            if (LoggedInUsers.Contains(user))
                throw new AlreadyLoggedInException(user);

            bool authenticated = Crypter.CheckPassword(password, GetUserHash(username));
            if (!authenticated)
                throw new InvalidCredentialsException(username);

            // Add user to the session
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
        public User GetUser(string username)
        {
            // Create safe query
            var sql = "SELECT * FROM users WHERE username = @username";
            // Prepare statement
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("username", username);

            // Communicate with database
            OrderedDictionary row = Database.Instance.SelectSingle(cmd);

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
        public User GetUser(Guid id)
        {
            // Create safe query
            var sql = "SELECT * FROM users WHERE id = @id";
            // Prepare statement
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", id);

            // Communicate with database
            OrderedDictionary row = Database.Instance.SelectSingle(cmd);

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
            return ParseHash(Database.Instance.SelectSingle(cmd));
        }

        public string GetUserHash(Guid id)
        {
            // Prepare sql
            var sql = "SELECT hash FROM users WHERE id=@id";
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", id);

            // Execute and process result
            return ParseHash(Database.Instance.SelectSingle(cmd));
        }

        /// <summary>
        /// Parses the user hash from the given row
        /// </summary>
        /// <param name="row">The row with the hash</param>
        /// <returns>The hash string if it was found in the row
        /// and an empty string if it was not</returns>
        private string ParseHash(OrderedDictionary row)
        {
            if (row == null) throw new NullReferenceException("The given row was null");
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
                { new SpellCard("WaterGoblin", "A water goblin", 23, Element.Fire, Rarity.Common), 5 }
            };
            return stack;
        }

        /// <summary>
        /// Buy command for the user for buying a card package
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="user"></param>
        /// <param name="package"></param>
        /// <returns>If the package buy action was successful</returns>
        public bool BuyPackage(HttpAuthorization auth, uint packageId)
        {
            // Check authorisazion
            User user = Authenticate(auth);

            if (user == null)
                return false;

            // Get package
            Package package = PackageController.Instance.GetPackage(packageId);

            if (package == null)
                return false;

            if (user.Coins < package.Cost)
                return false;

            user.Coins -= package.Cost;

            // Open the package
            List<CardInstance> drawnCards = PackageController.Instance.OpenPackage(package);

            // Add the cards to the user
            for (int i = 0; i < drawnCards.Count; i++)
            {
                AddCardToUser(user, drawnCards[i]);
            }

            return true;
        }

        public bool AddCardToUser(User user, CardInstance card)
        {
            if (user == null || card == null)
                return false;

            string sql = "INSERT INTO user_cards (userID, cardInstanceID) VALUES (@userID, @cardInstanceID);";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("userID", user.ID);
            cmd.Parameters.AddWithValue("cardInstanceID", card.ID);
            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }

        /// <summary>
        /// Returns the user by its auth token
        /// </summary>
        /// <param name="auth">The given auth token of the user</param>
        /// <returns>The user related to the auth token / null if no user was found</returns>
        public User Authenticate(HttpAuthorization auth)
        {
            // Get user by auth token
            User user = LoggedInUsers.Find(u => u.SessionToken == auth.Credentials);
            return user;
        }

        public void SetAdmin(Guid userID)
        {
            throw new NotImplementedException();
        }
    }
}
