using MTCG.Interfaces;
using MTCG.Models;
using MTCG.Serialization;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTCG.Controller
{
    public class CardController : Singleton<CardController>,
        ISelectable<Card>, 
        IDeletable<Card>, IDeleteMultiple<Card>,
        IInsertable<Card>, IInsertMultiple<Card>
    {
        public CardController() { }

        public bool Insert(Card card)
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

        /// <summary>
        /// Get a list of cards that are related to the given card instances
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public List<Card> GetCards(List<CardInstance> cardInstances)
        {
            List<Card> cards = new();
            NpgsqlCommand cmd = new("SELECT * FROM cards WHERE id=@card_id;");

            for (int i = 0; i < cardInstances.Count; i++)
            {
                cmd.Parameters.AddWithValue("card_id", cardInstances[i].CardID);
                var row = Database.Instance.SelectSingle(cmd);
                cmd.Parameters.Clear();

                cards.Add(Card.ParseFromDatabase(row));
            }
            return cards;
        }

        public bool Insert(List<Card> cards)
        {
            int errno = 0;

            for (int i = 0; i < cards.Count; i++)
            {
                errno += Insert(cards[i]) ? 0 : 1;
            }

            return errno == 0;
        }
        
        public Card Select(Guid cardID)
        {
            NpgsqlCommand cmd = new("SELECT * FROM cards WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", cardID);
            var row = Database.Instance.SelectSingle(cmd);

            return row != null ? Card.ParseFromDatabase(row) : null;
        }

        public bool Delete(Card card)
        {
            NpgsqlCommand cmd = new("DELETE FROM cards WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", card.ID);
            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }
        public bool Delete(List<Card> cards)
        {
            int errno = 0;

            for (int i = 0; i < cards.Count; i++)
            {
                errno += Delete(cards[i]) ? 0 : 1;
            }
            return errno == 0;
        }

        // Serialization
        public string GetDetailedCardsJson(List<CardInstance> cards)
        {
            CharStream s = new();
            s.Write("{ \"Cards\": [");

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

            Card card = Select(cardInstance.CardID);

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
        public string GetDetailedDeckJson(Deck deck)
        {
            CharStream s = new();
            s.Write("{");
            s.Write($"\"ID\":\"{deck.ID}\",");
            s.Write($"\"Name\":\"{deck.Name}\",");
            s.Write($"\"UserID\":\"{deck.UserID}\",");
            s.Write($"\"Cards\": [");

            for (int i = 0; i < deck.Cards.Count; i++)
            {
                s.Write(GetDetailedCardJson(deck.Cards[i]));
                if (i < deck.Cards.Count - 1)
                    s.Write(",");
            }

            s.Write("]}");
            return s.ToString();
        }
        public string GetDetailedDecksJson(List<Deck> decks)
        {
            CharStream s = new();
            s.Write("{ \"Decks\": [");

            for (int i = 0; i < decks.Count; i++)
            {
                s.Write(GetDetailedDeckJson(decks[i]));
                if (i < decks.Count - 1)
                    s.Write(",");
            }

            s.Write("]}");

            return s.ToString();
        }
    }
}
