using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
using Npgsql;

namespace MTCG.Controller
{
    class CardController : Singleton<CardController>
    {
        public bool InsertCard(Card card)
        {
            string sql = 
                @"INSERT INTO cards (ID, name, description, type, damage, element, rarity)
                  VALUES (@ID, @name, @description, @type, @damage, @element, @rarity)";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("ID", card.ID);
            cmd.Parameters.AddWithValue("name", card.Name);
            cmd.Parameters.AddWithValue("description", card.Description);
            cmd.Parameters.AddWithValue("type", (int)card.CardType);
            cmd.Parameters.AddWithValue("damage", card.Damage);
            cmd.Parameters.AddWithValue("element", (int)card.Element);
            cmd.Parameters.AddWithValue("rarity", (int)card.Rarity);

            return Server.Instance.Database.ExecuteNonQuery(cmd) == 1;
        }
        public Card GetCard(Guid cardID)
        {
            string sql = "SELECT * FROM cards WHERE ID=@ID;";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("ID", cardID.ToString());
            var row = Server.Instance.Database.SelectSingle(cmd);

            return row != null ? Card.ParseFromDatabase(row) : null;
        }

        /// <summary>
        /// Inserts a card instance into the database
        /// </summary>
        /// <param name="card"></param>
        /// <returns>If the insert was successful</returns>
        public bool InsertCardInstance(CardInstance card)
        {
            string sql = "INSERT INTO card_instances (ID, cardID) VALUES (@ID, @cardID);";
            var cmd = new Npgsql.NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("ID", card.ID);
            cmd.Parameters.AddWithValue("cardID", card.ID);
            return Server.Instance.Database.ExecuteNonQuery(cmd) == 1;
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
            string sql = "DELETE FROM card_instances WHERE ID = @ID;";
            var cmd = new Npgsql.NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("ID", card.ID);
            return Server.Instance.Database.ExecuteNonQuery(cmd) == 1;
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
