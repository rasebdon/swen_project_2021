using MTCG.Models;
using Npgsql;
using System.Collections.Specialized;

namespace MTCG.DAL.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly IDatabase _db;
        private readonly ILog _log;

        public UserRepository(IDatabase db, ILog log)
        {
            _db = db;
            _log = log;
        }

        public User? GetById(Guid id)
        {
            // Create safe query
            var sql = "SELECT * FROM users WHERE id=@id;";
            // Prepare statement
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", id);

            // Communicate with database
            return ParseFromRow(_db.SelectSingle(cmd), _log);
        }

        public User? GetByUsername(string username)
        {
            // Create safe query
            var sql = "SELECT * FROM users WHERE username=@username;";
            // Prepare statement
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("username", username);

            // Communicate with database
            return ParseFromRow(_db.SelectSingle(cmd), _log);
        }

        public bool Insert(User entity)
        {
            try
            {
                // Check if username is available (check in lower case)
                var cmd = new NpgsqlCommand(
                    $"SELECT username FROM users WHERE LOWER(username)=@username;");
                cmd.Parameters.AddWithValue("username", entity.Username.ToLower());

                User? selected = ParseFromRow(_db.SelectSingle(cmd), _log);
                if (selected != null && selected.Username == entity.Username)
                    throw new DuplicateEntryException(entity.Username);

                // Insert user
                string sql = $"INSERT INTO users (id, username, hash, coins, elo, admin, played_games) VALUES (@id, @username, @hash, @coins, @elo, false, @played_games);";
                cmd = new NpgsqlCommand(sql);
                cmd.Parameters.AddWithValue("id", entity.ID);
                cmd.Parameters.AddWithValue("username", entity.Username);
                cmd.Parameters.AddWithValue("hash", entity.Hash);
                cmd.Parameters.AddWithValue("coins", entity.Coins);
                cmd.Parameters.AddWithValue("elo", (int)entity.ELO);
                cmd.Parameters.AddWithValue("played_games", entity.PlayedGames);

                // Return if the database command affected exactly one row (inserted)
                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Updated the users information without changing id, username and password hash!
        /// </summary>
        /// <param name="entityOld"></param>
        /// <param name="entityNew"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Update(User entityOld, User entityNew)
        {
            try
            {
                string sql =
                    @"UPDATE users SET coins=@coins, elo=@elo, admin=@admin, played_games=@played_games
                WHERE id=@id;";
                NpgsqlCommand cmd = new(sql);
                cmd.Parameters.AddWithValue("id", entityOld.ID);
                cmd.Parameters.AddWithValue("coins", entityNew.Coins);
                cmd.Parameters.AddWithValue("elo", (int)entityNew.ELO);
                cmd.Parameters.AddWithValue("admin", entityNew.IsAdmin);
                cmd.Parameters.AddWithValue("played_games", entityNew.PlayedGames);

                // Return if the database command affected exactly one row (updated)
                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool Delete(User entity)
        {
            if (entity != null)
            {
                // Try delete by id
                NpgsqlCommand cmd = new("DELETE FROM users WHERE id=@id;");
                cmd.Parameters.AddWithValue("id", entity.ID);

                if (_db.ExecuteNonQuery(cmd) != 1)
                {
                    cmd = new("DELETE FROM users WHERE username=@username;");
                    cmd.Parameters.AddWithValue("username", entity.Username);

                    return _db.ExecuteNonQuery(cmd) == 1;
                }
                else return true;
            }
            return false;
        }

        public static User? ParseFromRow(OrderedDictionary? row, ILog log)
        {
            try
            {
                User? user = new(
                Guid.Parse(row?["id"]?.ToString() ?? ""),
                row?["username"]?.ToString() ?? "",
                row?["hash"]?.ToString() ?? "",
                int.Parse(row?["coins"]?.ToString() ?? ""),
                ushort.Parse(row?["elo"]?.ToString() ?? ""),
                int.Parse(row?["played_games"]?.ToString() ?? ""));

                return user;
            }
            catch (Exception ex)
            {
                log.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
