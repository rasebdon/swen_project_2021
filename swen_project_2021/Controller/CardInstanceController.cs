using MTCG.Interfaces;
using MTCG.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Controller
{
    public class CardInstanceController : Singleton<CardInstanceController>,
        ISelectable<CardInstance>,
        IDeletable<CardInstance>, IDeleteMultiple<CardInstance>,
        IInsertable<CardInstance>, IInsertMultiple<CardInstance>
    {
        public CardInstance Select(Guid id)
        {

            NpgsqlCommand cmd = new("SELECT * FROM card_instances WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", id);
            return new CardInstance(Database.Instance.SelectSingle(cmd));

        }

        /// <summary>
        /// Inserts a card instance into the database
        /// </summary>
        /// <param name="card"></param>
        /// <returns>If the insert was successful</returns>
        public bool Insert(CardInstance card)
        {
            string sql = "INSERT INTO card_instances (id, card_id) VALUES (@id, @cardID);";
            var cmd = new Npgsql.NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", card.ID);
            cmd.Parameters.AddWithValue("cardID", card.CardID);
            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }
        /// <summary>
        /// Inserts a bunch of cards into the database
        /// </summary>
        /// <param name="cards"></param>
        /// <returns>If the all the inserts were successful</returns>
        public bool Insert(List<CardInstance> cards)
        {
            int err = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                err += Insert(cards[i]) ? 0 : 1;
            }
            return err == 0;
        }

        public bool Delete(CardInstance card)
        {
            string sql = "DELETE FROM card_instances WHERE id = @id;";
            var cmd = new Npgsql.NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", card.ID);
            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }
        public bool Delete(List<CardInstance> cards)
        {
            int err = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                err += Delete(cards[i]) ? 0 : 1;
            }
            return err == 0;
        }
}
}
