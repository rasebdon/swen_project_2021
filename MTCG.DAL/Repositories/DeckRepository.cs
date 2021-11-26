using MTCG.Models;
using Npgsql;
using System.Collections;
using System.Collections.Specialized;

namespace MTCG.DAL.Repositories
{
    public class DeckRepository : IRepository<Deck>
    {
        private readonly StackRepository _stackRepository;
        private readonly IDatabase _db;
        private readonly ILog _log;

        public DeckRepository(IDatabase db, StackRepository stackRepository, ILog log)
        {
            _stackRepository = stackRepository;
            _db = db;
            _log = log;
        }

        public bool Delete(Deck deck)
        {
            NpgsqlCommand cmd = new("DELETE FROM decks WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", deck.ID);
            return _db.ExecuteNonQuery(cmd) == 1;
        }

        public IEnumerable GetAll()
        {
            throw new NotImplementedException();
        }

        public Deck? GetById(Guid deckid)
        {
            try
            {
                // Get deck info
                NpgsqlCommand cmd = new(
                @"SELECT * FROM decks, user_decks WHERE id=@id AND deck_id=id;");
                cmd.Parameters.AddWithValue("id", deckid);
                OrderedDictionary deckInfo = _db.SelectSingle(cmd);

                // Get card infos extension 
                cmd = new(
                    @"SELECT card_instances.* FROM deck_cards, card_instances 
                WHERE deck_cards.deck_id=@deckId AND card_instances.id=deck_cards.card_instance_id");
                cmd.Parameters.AddWithValue("deckId", deckid);
                OrderedDictionary[] cards = _db.Select(cmd);

                return ParseFromRow(deckInfo, cards);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<Deck> GetUserDecks(Guid userId)
        {
            try
            {
                List<Deck>? decks = new();

                // Get the decks information
                NpgsqlCommand cmd = new(
                    @"SELECT decks.*, user_id, main_deck
                FROM decks, user_decks
                WHERE user_id=@userId
                AND deck_id=id;");
                cmd.Parameters.AddWithValue("userId", userId);
                OrderedDictionary[] deckInformations = _db.Select(cmd);

                foreach (OrderedDictionary row in deckInformations)
                {
                    Deck? deck = GetById(Guid.Parse(row["id"]?.ToString() ?? ""));
                    if (deck != null) decks.Add(deck);
                }

                return decks;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return new();
            }
        }

        public bool Insert(Deck deck)
        {
            NpgsqlCommand cmd;
            Guid oldMainDeckId = Guid.Empty;

            try
            {
                bool mainDeck;

                if (!deck.MainDeck)
                {
                    // Check if it is the first deck of the user => make it the main deck
                    mainDeck = GetUserDecks(deck.UserID).Count == 0;
                }
                else
                {
                    // Get old main deck
                    cmd = new("SELECT * FROM decks, user_decks WHERE user_id=@user_id AND main_deck=true;");
                    cmd.Parameters.AddWithValue("user_id", deck.UserID);
                    oldMainDeckId = Guid.Parse(_db.SelectSingle(cmd)?["id"]?.ToString() ?? "");

                    // Uncheck old main deck
                    cmd = new("UPDATE user_decks SET main_deck=False WHERE main_deck=True AND user_id=@user_id;");
                    cmd.Parameters.AddWithValue("user_id", deck.UserID);
                    _db.ExecuteNonQuery(cmd);

                    // Set this deck as new main deck
                    mainDeck = true;
                }

                int errorno = 0;
                // Insert deck metadata
                cmd = new("INSERT INTO decks (id, name) VALUES (@id, @name);");
                cmd.Parameters.AddWithValue("id", deck.ID);
                cmd.Parameters.AddWithValue("name", deck.Name);
                errorno += _db.ExecuteNonQuery(cmd) == 1 ? 0 : 1;

                // Insert user link
                cmd = new("INSERT INTO user_decks (deck_id, user_id, main_deck) VALUES (@deck_id, @user_id, @main_deck);");
                cmd.Parameters.AddWithValue("deck_id", deck.ID);
                cmd.Parameters.AddWithValue("user_id", deck.UserID);
                cmd.Parameters.AddWithValue("main_deck", mainDeck);
                errorno += _db.ExecuteNonQuery(cmd) == 1 ? 0 : 1;

                // Insert card links
                cmd = new("INSERT INTO deck_cards (deck_id, card_instance_id) VALUES (@deck_id, @card_instance_id);");
                cmd.Parameters.AddWithValue("deck_id", deck.ID);

                for (int i = 0; i < deck.Cards.Count; i++)
                {
                    cmd.Parameters.AddWithValue("card_instance_id", deck.Cards[i].ID);
                    errorno += _db.ExecuteNonQuery(cmd) == 1 ? 0 : 1;
                    cmd.Parameters.Remove("card_instance_id");
                }

                if (errorno != 0)
                {
                    throw new Exception($"There was an error inserting the new deck with id {deck.ID}");
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                Delete(deck);

                if (oldMainDeckId != Guid.Empty)
                {
                    // Set old deck to main deck
                    cmd = new("UPDATE user_decks SET main_deck=True WHERE id=@id;");
                    cmd.Parameters.AddWithValue("id", oldMainDeckId);
                    _db.ExecuteNonQuery(cmd);
                }
                return false;
            }
        }

        public bool Update(Deck oldDeck, Deck newDeck)
        {
            try
            {
                NpgsqlCommand cmd;
                bool mainDeck;
                if (!newDeck.MainDeck)
                {
                    // Check if it is the first deck of the user => make it the main deck
                    mainDeck = GetUserDecks(newDeck.UserID).Count == 0;
                }
                else
                {
                    // Uncheck old main deck
                    cmd = new("UPDATE user_decks SET main_deck=False WHERE main_deck=True AND user_id=@user_id");
                    cmd.Parameters.AddWithValue("user_id", newDeck.UserID);
                    _db.ExecuteNonQuery(cmd);

                    // Set this deck as new main deck
                    mainDeck = true;
                }

                // Update main deck
                if (mainDeck)
                {
                    cmd = new("UPDATE user_decks SET main_deck=True WHERE user_id=@user_id AND deck_id=@deck_id");
                    cmd.Parameters.AddWithValue("user_id", newDeck.UserID);
                    cmd.Parameters.AddWithValue("deck_id", oldDeck.ID);
                    _db.ExecuteNonQuery(cmd);
                }

                // Update deck information
                if (oldDeck.Name != newDeck.Name)
                {
                    cmd = new("UPDATE decks SET name=@name;");
                    cmd.Parameters.AddWithValue("name", newDeck.Name);
                    _db.ExecuteNonQuery(cmd);
                }

                // Update cards
                // Unlink old cards
                cmd = new("DELETE FROM deck_cards WHERE deck_id=@deck_id;");
                cmd.Parameters.AddWithValue("deck_id", oldDeck.ID);
                _db.ExecuteNonQuery(cmd);

                // Link new cards
                cmd = new("INSERT INTO deck_cards (deck_id, card_instance_id) VALUES (@deck_id, @card_instance_id);");
                cmd.Parameters.AddWithValue("deck_id", oldDeck.ID);
                for (int i = 0; i < newDeck.Cards.Count; i++)
                {
                    cmd.Parameters.AddWithValue("card_instance_id", newDeck.Cards[i].ID);
                    _db.ExecuteNonQuery(cmd);
                    cmd.Parameters.Remove("card_instance_id");
                }
            }
            catch (Exception e)
            {
                _log.WriteLine(e.ToString());
                // Revert changes
                Delete(oldDeck);
                Insert(oldDeck);
                return false;
            }
            return true;
        }

        private Deck? ParseFromRow(OrderedDictionary deckInfo, OrderedDictionary[] cardRows)
        {
            try
            {
                List<CardInstance> cards = new();
                foreach (OrderedDictionary row in cardRows)
                {
                    CardInstance? card = CardInstanceRepository.ParseFromRow(row);
                    if (card != null)
                        cards.Add(card);
                }

                return new Deck(
                    Guid.Parse(deckInfo?["id"]?.ToString() ?? ""),
                    deckInfo?["name"]?.ToString() ?? "",
                    Guid.Parse(deckInfo?["user_id"]?.ToString() ?? ""),
                    bool.Parse(deckInfo?["main_deck"]?.ToString() ?? ""),
                    cards.ToArray());
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
