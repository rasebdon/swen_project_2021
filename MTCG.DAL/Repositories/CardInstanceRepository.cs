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

        public IEnumerable<CardInstance> GetAll()
        {
            OrderedDictionary[] rows = _db.Select(
                new NpgsqlCommand(@"SELECT card_instances.*, cards.type, cards.name, cards.description,
                cards.damage, cards.element, cards.rarity, cards.race  FROM card_instances, cards
                WHERE card_id=cards.id;"));

            List<CardInstance> list = new();

            foreach (var row in rows)
            {
                CardInstance? cardInstance = ParseFromRow(row, _log);
                if(cardInstance != null)
                    list.Add(cardInstance);
            }

            return list;
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

        public bool Delete(Guid id)
        {
            try
            {
                string sql = "DELETE FROM card_instances WHERE id = @id;";
                var cmd = new Npgsql.NpgsqlCommand(sql);
                cmd.Parameters.AddWithValue("id", id);
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
                NpgsqlCommand cmd = new(@"SELECT card_instances.*, cards.type, cards.name, cards.description,
                cards.damage, cards.element, cards.rarity, cards.race  FROM card_instances, cards
                WHERE card_id=cards.id AND card_instances.id=@id;");
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

                return _db.ExecuteNonQueryTransaction(new TransactionObject[] { new TransactionObject(cmd, 1) });
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
                List<TransactionObject> transaction = new();
                List<CardInstance> addedCards = new();

                foreach (CardInstance cardInstance in cards)
                {
                    string sql =
                        @"INSERT INTO user_cards (user_id, card_instance_id) 
                        VALUES (@user_id, @card_instance_id);";
                    NpgsqlCommand cmd = new(sql);
                    cmd.Parameters.AddWithValue("user_id", user.ID);
                    cmd.Parameters.AddWithValue("card_instance_id", cardInstance.ID);
                    transaction.Add(new(cmd, 1));

                    addedCards.Add(cardInstance);
                }

                return _db.ExecuteNonQueryTransaction(transaction);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool Insert(List<CardInstance> cards)
        {
            try
            {
                List<TransactionObject> transaction = new();

                foreach (CardInstance cardInstance in cards)
                {
                    var cmd = new NpgsqlCommand(
                    "INSERT INTO card_instances (id, card_id) VALUES (@id, @cardID);");
                    cmd.Parameters.AddWithValue("id", cardInstance.ID);
                    cmd.Parameters.AddWithValue("cardID", cardInstance.CardID);
                    transaction.Add(new(cmd, 1));
                }
                return _db.ExecuteNonQueryTransaction(transaction);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool Update(CardInstance entity)
        {
            try
            {
                string sql = "UPDATE card_instances SET card_id=@card_id WHERE id=@id;";
                NpgsqlCommand cmd = new(sql);
                cmd.Parameters.AddWithValue("id", entity.ID);
                cmd.Parameters.AddWithValue("card_id", entity.CardID);

                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
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
                    Enum.Parse<CardType>(row?["type"]?.ToString() ?? ""),
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
