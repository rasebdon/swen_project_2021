using MTCG.Models;
using Npgsql;
using System.Collections;
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
            var cmd = new NpgsqlCommand(
                "INSERT INTO card_instances (id, card_id) VALUES (@id, @cardID);");
            cmd.Parameters.AddWithValue("id", card.ID);
            cmd.Parameters.AddWithValue("cardID", card.CardID);
            return _db.ExecuteNonQuery(cmd) == 1;
        }

        public bool Delete(CardInstance card)
        {
            string sql = "DELETE FROM card_instances WHERE id = @id;";
            var cmd = new Npgsql.NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", card.ID);
            return _db.ExecuteNonQuery(cmd) == 1;
        }

        public IEnumerable GetAll()
        {
            throw new NotImplementedException();
        }

        public CardInstance? GetById(Guid id)
        {
            NpgsqlCommand cmd = new("SELECT * FROM card_instances WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", id);

            OrderedDictionary row = _db.SelectSingle(cmd);

            return ParseFromRow(row);
        }

        public bool AddCardInstanceToUser(User user, CardInstance card)
        {
            string sql = 
                @"INSERT INTO user_cards (user_id, card_instance_id) 
                VALUES (@user_id, @card_instance_id);";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("user_id", user.ID);
            cmd.Parameters.AddWithValue("card_instance_id", card.ID);

            if(_db.ExecuteNonQuery(cmd) != 1)
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
        public bool AddCardInstancesToUser(User user, List<CardInstance> cards)
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
                if(_db.ExecuteNonQuery(cmd) != 1)
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

        public bool Update(CardInstance entityOld, CardInstance entityNew)
        {
            throw new NotImplementedException();
        }

        public static CardInstance? ParseFromRow(OrderedDictionary row)
        {
            return new(
                Guid.Parse(row?["id"]?.ToString() ?? ""),
                Guid.Parse(row?["card_id"]?.ToString() ?? ""),
                row?["name"]?.ToString() ?? "",
                row?["description"]?.ToString() ?? "",
                int.Parse(row?["damage"]?.ToString() ?? ""),
                Enum.Parse<CardType>(row?["card_type"]?.ToString() ?? ""),
                Enum.Parse<Element>(row?["element"]?.ToString() ?? ""),
                Enum.Parse<Race>(row?["race"]?.ToString() ?? ""),
                Enum.Parse<Rarity>(row?["rarity"]?.ToString() ?? ""));
        }
    }
}
