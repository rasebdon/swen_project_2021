using MTCG.Models;
using MTCG.Serialization;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTCG.Controller
{
    public class CardController : Singleton<CardController>
    {
        public CardController() { }

        public bool InsertCard(Card card)
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
            int raceVal = card.CardType == CardType.Monster ? (int)((MonsterCard)card).Race : 0;
            cmd.Parameters.AddWithValue("race", raceVal);

            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }
        public bool InsertCards(List<Card> cards)
        {
            int errno = 0;

            for (int i = 0; i < cards.Count; i++)
            {
                errno += InsertCard(cards[i]) ? 0 : 1;
            }

            return errno == 0;
        }
        public Card GetCard(Guid cardID)
        {
            NpgsqlCommand cmd = new("SELECT * FROM cards WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", cardID);
            var row = Database.Instance.SelectSingle(cmd);

            return row != null ? Card.ParseFromDatabase(row) : null;
        }

        public CardInstance GetCardInstance(Guid cardInstanceID)
        {
            NpgsqlCommand cmd = new("SELECT * FROM card_instances WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", cardInstanceID);
            return new CardInstance(Database.Instance.SelectSingle(cmd));
        }

        public bool DeleteCard(Card card)
        {
            NpgsqlCommand cmd = new("DELETE FROM cards WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", card.ID);
            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }
        public bool DeleteCards(List<Card> cards)
        {
            int errno = 0;

            for (int i = 0; i < cards.Count; i++)
            {
                errno += DeleteCard(cards[i]) ? 0 : 1;
            }
            return errno == 0;
        }

        /// <summary>
        /// Inserts a card instance into the database
        /// </summary>
        /// <param name="card"></param>
        /// <returns>If the insert was successful</returns>
        public bool InsertCardInstance(CardInstance card)
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


        public string GetDetailedCardsJson(List<CardInstance> cards)
        {
            CharStream s = new();
            s.Write("{ Cards: [");

            for (int i = 0; i < cards.Count; i++)
            {
                s.Write(GetDetailedCardJson(cards[i]));
                if (i < cards.Count - 1)
                    s.Write(",");
            }

            s.Write("]}");

            return s.ToString();
        }
        public string GetDetailedCardJson(CardInstance cardInstance)
        {
            CharStream s = new();
            s.Write($"{{\"CardInstanceID\":\"{cardInstance.ID}\",");

            Card card = GetCard(cardInstance.CardID);

            s.Write($"\"CardID\":\"{card.ID}\",");
            s.Write($"\"Name\":\"{card.Name}\",");
            s.Write($"\"Description\":\"{card.Description}\",");
            s.Write($"\"CardType\":\"{card.CardType}\",");
            s.Write($"\"Damage\":\"{card.Damage}\",");
            s.Write($"\"Element\":\"{card.Element}\",");
            s.Write($"\"Rarity\":\"{card.Rarity}\"{ (card.CardType == CardType.Monster ? "," : "") }");
            if(card.CardType == CardType.Monster)
                s.Write($"\"Race\":\"{(card as MonsterCard).Race}\",");
            s.Write("}");
            return s.ToString();
        }
    }
}
