using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
using Npgsql;

namespace MTCG.Controller
{
    public class CardController : Singleton<CardController>
    {
        public CardController() { }

        public bool InsertCard(Card card)
        {
            string sql =
                @"INSERT INTO cards (id, name, description, type, damage, element, rarity)
                  VALUES (@id, @name, @description, @type, @damage, @element, @rarity)";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("id", card.ID);
            cmd.Parameters.AddWithValue("name", card.Name);
            cmd.Parameters.AddWithValue("description", card.Description);
            cmd.Parameters.AddWithValue("type", (int)card.CardType);
            cmd.Parameters.AddWithValue("damage", card.Damage);
            cmd.Parameters.AddWithValue("element", (int)card.Element);
            cmd.Parameters.AddWithValue("rarity", (int)card.Rarity);

            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }
        public Card GetCard(Guid cardID)
        {
            string sql = "SELECT * FROM cards WHERE id=@id;";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("id", cardID);
            var row = Database.Instance.SelectSingle(cmd);

            return row != null ? Card.ParseFromDatabase(row) : null;
        }

        /// <summary>
        /// Inserts a card instance into the database
        /// </summary>
        /// <param name="card"></param>
        /// <returns>If the insert was successful</returns>
        public bool InsertCardInstance(CardInstance card)
        {
            string sql = "INSERT INTO card_instances (id, cardID) VALUES (@id, @cardID);";
            var cmd = new Npgsql.NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", card.ID);
            cmd.Parameters.AddWithValue("cardID", card.ID);
            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }
        /// <summary>
        /// Inserts a bunch of cards into the database
        /// </summary>
        /// <param name="cards"></param>
        /// <returns>If the all the inserts were successful</returns>
        public bool InsertCardInstances(List<CardInstance> cards)
        {
            int err = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                err += InsertCardInstance(cards[i]) ? 0 : 1;
            }
            return err == 0;
        }

        public bool DeleteCardInstance(CardInstance card)
        {
            string sql = "DELETE FROM card_instances WHERE id = @id;";
            var cmd = new Npgsql.NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", card.ID);
            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }
        public bool DeleteCardInstances(List<CardInstance> cards)
        {
            int err = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                err += DeleteCardInstance(cards[i]) ? 0 : 1;
            }
            return err == 0;
        }
    }
}
