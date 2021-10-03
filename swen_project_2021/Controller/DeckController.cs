using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interfaces;
using MTCG.Models;
using Npgsql;

namespace MTCG.Controller
{
    public class DeckController : Singleton<DeckController>, ISelectable<Deck>, IDeletable<Deck>, IInsertable<Deck>
    {
        public Deck Select(Guid deckId)
        {
            NpgsqlCommand cmd = new("SELECT * FROM decks, user_decks WHERE id=@id AND deck_id=id;");
            cmd.Parameters.AddWithValue("id", deckId);
            var deckInfoRow = Database.Instance.SelectSingle(cmd);

            cmd = new("SELECT card_instances.* FROM deck_cards, card_instances WHERE deck_cards.deck_id=@deckId AND card_instances.id=deck_cards.card_instance_id");
            cmd.Parameters.AddWithValue("deckId", deckId);
            var deckCardsRow = Database.Instance.Select(cmd);

            return new Deck(deckInfoRow, deckCardsRow);
        }
        public bool Delete(Deck deck)
        {
            if (deck == null)
                return false;
            NpgsqlCommand cmd = new("DELETE FROM decks WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", deck.ID);
            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }

        public bool Insert(Deck deck)
        {
            if (deck == null)
                throw new NullReferenceException("Deck cannot be null!");

            if (deck.Cards.Count != Deck.DeckSize)
                throw new ArgumentException("Invalid deck size of deck!");

            // Check if cards belong to the user
            List<CardInstance> cards = new();
            List<CardInstance> stack = UserController.Instance.GetUserCardStack(deck.UserID);
            for (int i = 0; i < stack.Count; i++)
            {
                for (int j = 0; j < deck.Cards.Count; j++)
                {
                    if (stack[i].ID == deck.Cards[j].ID)
                    {
                        if (cards.Count >= 4)
                            throw new ArgumentException("There can be no deck with more than 4 cards!");

                        if (cards.Find(c => c.ID == stack[i].ID) != null)
                            throw new ArgumentException("Each card can only be in a deck one time!");

                        cards.Add(stack[i]);
                    }
                }
            }
            if (cards.Count != Deck.DeckSize)
                throw new ArgumentException("Some cards in the deck do not belong to the user!");

            int errorno = 0;
            // Insert deck metadata
            NpgsqlCommand cmd = new("INSERT INTO decks (id, name) VALUES (@id, @name);");
            cmd.Parameters.AddWithValue("id", deck.ID);
            cmd.Parameters.AddWithValue("name", deck.Name);
            errorno += Database.Instance.ExecuteNonQuery(cmd) == 1 ? 0 : 1;

            // Insert user link
            cmd = new("INSERT INTO user_decks (deck_id, user_id) VALUES (@deck_id, @user_id);");
            cmd.Parameters.AddWithValue("deck_id", deck.ID);
            cmd.Parameters.AddWithValue("user_id", deck.UserID);
            errorno += Database.Instance.ExecuteNonQuery(cmd) == 1 ? 0 : 1;

            // Insert card links
            cmd = new("INSERT INTO deck_cards (deck_id, card_instance_id) VALUES (@deck_id, @card_instance_id);");
            cmd.Parameters.AddWithValue("deck_id", deck.ID);
            for (int i = 0; i < deck.Cards.Count; i++)
            {
                cmd.Parameters.AddWithValue("card_instance_id", deck.Cards[i].ID);
                errorno += Database.Instance.ExecuteNonQuery(cmd) == 1 ? 0 : 1;
                cmd.Parameters.Remove("card_instance_id");
            }

            if (errorno != 0)
            {
                Delete(deck);
                return false;
            }
            return true;
        }
    
        //public bool Update(Deck oldDeck, Deck newDeck)
        //{
        //    if (oldDeck == null || newDeck == null)
        //        throw new NullReferenceException("Old or new dack cannot be null!");

        //    NpgsqlCommand
        //}
    }
}
