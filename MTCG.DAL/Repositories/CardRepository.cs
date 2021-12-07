using MTCG.Models;
using Npgsql;
using System.Collections.Specialized;

namespace MTCG.DAL.Repositories
{
    public class CardRepository : IRepository<Card>
    {
        private readonly IDatabase _db;
        private readonly ILog _log;

        public CardRepository(IDatabase db, ILog log)
        {
            _db = db;
            _log = log;
        }

        public IEnumerable<Card> GetAll()
        {
            OrderedDictionary[] rows = _db.Select(
                new NpgsqlCommand("SELECT * FROM cards;"));

            List<Card> list = new();

            foreach (var row in rows)
            {
                Card? card = ParseFromRow(row, _log);
                if (card != null)
                    list.Add(card);
            }

            return list;
        }

        public bool Delete(Guid id)
        {
            NpgsqlCommand cmd = new("DELETE FROM cards WHERE id=@id");
            cmd.Parameters.AddWithValue("id", id);
            return _db.ExecuteNonQuery(cmd) == 1;
        }

        public Card? GetById(Guid id)
        {
            NpgsqlCommand cmd = new("SELECT * FROM cards WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", id);
            OrderedDictionary row = _db.SelectSingle(cmd);

            return ParseFromRow(row, _log);
        }

        public bool Insert(Card card)
        {
            try
            {
                string sql =
                    @"INSERT INTO cards (id, name, description, type, damage, element, rarity, race)
                    VALUES (@id, @name, @description, @type, @damage, @element, @rarity, @race)";
                NpgsqlCommand cmd = new(sql);
                cmd.Parameters.AddWithValue("id", card.ID);
                cmd.Parameters.AddWithValue("name", card.Name);
                cmd.Parameters.AddWithValue("description", card.Description);
                cmd.Parameters.AddWithValue("type", (int)card.CardType);
                cmd.Parameters.AddWithValue("damage", card.Damage);
                cmd.Parameters.AddWithValue("element", (int)card.Element);
                cmd.Parameters.AddWithValue("rarity", (int)card.Rarity);
                cmd.Parameters.AddWithValue("race", (int)card.CardType);

                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool Update(Card entity)
        {
            try
            {
                string sql =
                    @"UPDATE cards 
                    SET name=@name, description=@description, type=@type, damage=@damage, 
                    element=@element, rarity=@rarity, race=@race
                    WHERE id=@id;";
                NpgsqlCommand cmd = new(sql);
                cmd.Parameters.AddWithValue("id", entity.ID);
                cmd.Parameters.AddWithValue("name", entity.Name);
                cmd.Parameters.AddWithValue("description", entity.Description);
                cmd.Parameters.AddWithValue("type", (int)entity.CardType);
                cmd.Parameters.AddWithValue("damage", entity.Damage);
                cmd.Parameters.AddWithValue("element", (int)entity.Element);
                cmd.Parameters.AddWithValue("rarity", (int)entity.Rarity);
                cmd.Parameters.AddWithValue("race", (int)entity.CardType);

                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }


        public static Card? ParseFromRow(OrderedDictionary row, ILog log)
        {
            try
            {
                return new Card(
                Guid.Parse(row?["id"]?.ToString() ?? ""),
                row?["name"]?.ToString() ?? "",
                row?["description"]?.ToString() ?? "",
                int.Parse(row?["damage"]?.ToString() ?? ""),
                Enum.Parse<CardType>(row?["type"]?.ToString() ?? ""),
                Enum.Parse<Element>(row?["element"]?.ToString() ?? ""),
                Enum.Parse<Race>(row?["race"]?.ToString() ?? ""),
                Enum.Parse<Rarity>(row?["rarity"]?.ToString() ?? ""));
            }
            catch (Exception ex)
            {
                log.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
