using MTCG.Models;
using Npgsql;
using System.Collections.Specialized;

namespace MTCG.DAL.Repositories
{
    public class CardInstanceRepository : IRepository<CardInstance>
    {
        private readonly IDatabase _db;
        private readonly ILog _log;

        public CardInstanceRepository(IDatabase db, ILog log)
        {
            _db = db;
            _log = log;
        }

        /// <summary>
        /// Inserts a card instance into the database
        /// </summary>
        /// <param name="card"></param>
        /// <returns>If the insert was successful</returns>
        public bool Insert(CardInstance card)
        {
            try
            {
                var cmd = new NpgsqlCommand(
                "INSERT INTO card_instances (id, card_id) VALUES (@id, @cardID);");
                cmd.Parameters.AddWithValue("id", card.ID);
                cmd.Parameters.AddWithValue("cardID", card.CardID);
                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool Delete(CardInstance card)
        {
            try
            {
                string sql = "DELETE FROM card_instances WHERE id = @id;";
                var cmd = new Npgsql.NpgsqlCommand(sql);
                cmd.Parameters.AddWithValue("id", card.ID);
                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public CardInstance? GetById(Guid id)
        {
            try
            {
                NpgsqlCommand cmd = new("SELECT * FROM card_instances WHERE id=@id;");
                cmd.Parameters.AddWithValue("id", id);
                return ParseFromRow(_db.SelectSingle(cmd), _log);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return null;
            }
        }

        public bool AddCardInstanceToUser(User user, CardInstance card)
        {
            try
            {
                string sql =
                @"INSERT INTO user_cards (user_id, card_instance_id) 
                VALUES (@user_id, @card_instance_id);";
                NpgsqlCommand cmd = new(sql);
                cmd.Parameters.AddWithValue("user_id", user.ID);
                cmd.Parameters.AddWithValue("card_instance_id", card.ID);

                if (_db.ExecuteNonQuery(cmd) != 1)
                {
                    // Revert changes
                    cmd = new(
                        @"DELETE FROM user_cards WHERE 
                        user_id=@user_id AND card_instance_id=@card_instance_id");
                    cmd.Parameters.AddWithValue("user_id", user.ID);
                    cmd.Parameters.AddWithValue("card_instance_id", card.ID);

                    _db.ExecuteNonQuery(cmd);

                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }
        public bool AddCardInstancesToUser(User user, List<CardInstance> cards)
        {
            try
            {
                string sql =
                @"INSERT INTO user_cards (user_id, card_instance_id) 
                VALUES (@user_id, @card_instance_id);";
                NpgsqlCommand cmd = new(sql);
                cmd.Parameters.Add("user_id", NpgsqlTypes.NpgsqlDbType.Uuid);
                cmd.Parameters.Add("card_instance_id", NpgsqlTypes.NpgsqlDbType.Uuid);

                List<CardInstance> addedCards = new();

                foreach (CardInstance card in cards)
                {
                    cmd.Parameters["user_id"].Value = user.ID;
                    cmd.Parameters["card_instance_id"].Value = card.ID;

                    // Check if transaction was successful
                    if (_db.ExecuteNonQuery(cmd) != 1)
                    {
                        // Revert changes
                        cmd = new(
                            @"DELETE FROM user_cards WHERE 
                            user_id=@user_id AND card_instance_id=@card_instance_id");
                        cmd.Parameters.Add("user_id", NpgsqlTypes.NpgsqlDbType.Uuid);
                        cmd.Parameters.Add("card_instance_id", NpgsqlTypes.NpgsqlDbType.Uuid);

                        foreach (CardInstance c in addedCards)
                        {
                            cmd.Parameters["user_id"].Value = user.ID;
                            cmd.Parameters["card_instance_id"].Value = c.ID;

                            _db.ExecuteNonQuery(cmd);
                        }
                        return false;
                    }
                    addedCards.Add(card);
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool Update(CardInstance entityOld, CardInstance entityNew)
        {
            try
            {
                string sql = "UPDATE card_instances SET (card_id) VALUES (@card_id) WHERE id=@id;";
                NpgsqlCommand cmd = new(sql);
                cmd.Parameters.AddWithValue("id", entityOld.ID);
                cmd.Parameters.AddWithValue("card_id", entityNew.CardID);

                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch(Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public static CardInstance? ParseFromRow(OrderedDictionary row, ILog log)
        {
            try
            {
                CardInstance instance = new(
                    Guid.Parse(row?["id"]?.ToString() ?? ""),
                    Guid.Parse(row?["card_id"]?.ToString() ?? ""),
                    row?["name"]?.ToString() ?? "",
                    row?["description"]?.ToString() ?? "",
                    int.Parse(row?["damage"]?.ToString() ?? ""),
                    Enum.Parse<CardType>(row?["card_type"]?.ToString() ?? ""),
                    Enum.Parse<Element>(row?["element"]?.ToString() ?? ""),
                    Enum.Parse<Race>(row?["race"]?.ToString() ?? ""),
                    Enum.Parse<Rarity>(row?["rarity"]?.ToString() ?? ""));

                return instance;
            }
            catch (Exception ex)
            {
                log.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
