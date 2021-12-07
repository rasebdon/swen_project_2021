using MTCG.Models;
using Npgsql;
using System.Collections.Specialized;

namespace MTCG.DAL.Repositories
{
    public class DeckRepository : IRepository<Deck>
    {
        private readonly IDatabase _db;
        private readonly ILog _log;

        public DeckRepository(IDatabase db, ILog log)
        {
            _db = db;
            _log = log;
        }

        public IEnumerable<Deck> GetAll()
        {
            // Get decks
            OrderedDictionary[] deckRows = _db.Select(
                new NpgsqlCommand("SELECT * FROM decks, user_decks WHERE deck_id=id;"));

            List<Deck> decks = new();

            foreach (var row in deckRows)
            {
                // Get card infos extension 
                NpgsqlCommand cmd = new(
                    @$"SELECT card_instances.*, cards.type, cards.name, cards.description,
                    cards.damage, cards.element, cards.rarity, cards.race FROM card_instances, cards, decks, deck_cards
                    WHERE card_instances.card_id=cards.id
                    AND deck_cards.deck_id=decks.id
                    AND deck_cards.card_instance_id=card_instances.id
                    AND decks.id={row["deck_id"]};");
                OrderedDictionary[] cards = _db.Select(cmd);

                Deck? deck = ParseFromRow(row, cards, _log);
                if (deck != null)
                    decks.Add(deck);
            }

            return decks;
        }

        public bool Delete(Guid id)
        {
            NpgsqlCommand cmd = new("DELETE FROM decks WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", id);
            return _db.ExecuteNonQuery(cmd) == 1;
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
                    @"SELECT card_instances.*, cards.type, cards.name, cards.description,
                    cards.damage, cards.element, cards.rarity, cards.race FROM card_instances, cards, decks, deck_cards
                    WHERE card_instances.card_id=cards.id
                    AND deck_cards.deck_id=decks.id
                    AND deck_cards.card_instance_id=card_instances.id
                    AND decks.id=@deckId;");
                cmd.Parameters.AddWithValue("deckId", deckid);
                OrderedDictionary[] cards = _db.Select(cmd);

                return ParseFromRow(deckInfo, cards, _log);
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
                    if (deck != null)
                        decks.Add(deck);
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
            List<TransactionObject> commands = new();

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
                    NpgsqlCommand cmd = new("SELECT * FROM decks, user_decks WHERE user_id=@user_id AND main_deck=true;");
                    cmd.Parameters.AddWithValue("user_id", deck.UserID);

                    if (Guid.TryParse(_db.SelectSingle(cmd)?["id"]?.ToString() ?? "", out Guid oldMainDeckId))
                    {
                        // Uncheck old main deck
                        NpgsqlCommand cmd1 = new("UPDATE user_decks SET main_deck=False WHERE main_deck=True AND user_id=@user_id;");
                        cmd1.Parameters.AddWithValue("user_id", deck.UserID);
                        commands.Add(new(cmd1, 1));
                    }

                    // Set this deck as new main deck
                    mainDeck = true;
                }

                // Insert deck metadata
                NpgsqlCommand cmd2 = new("INSERT INTO decks (id, name) VALUES (@id, @name);");
                cmd2.Parameters.AddWithValue("id", deck.ID);
                cmd2.Parameters.AddWithValue("name", deck.Name);
                commands.Add(new(cmd2, 1));

                // Insert user link
                NpgsqlCommand cmd3 = new("INSERT INTO user_decks (deck_id, user_id, main_deck) VALUES (@deck_id, @user_id, @main_deck);");
                cmd3.Parameters.AddWithValue("deck_id", deck.ID);
                cmd3.Parameters.AddWithValue("user_id", deck.UserID);
                cmd3.Parameters.AddWithValue("main_deck", mainDeck);
                commands.Add(new(cmd3, 1));

                // Insert card links
                for (int i = 0; i < deck.Cards.Count; i++)
                {
                    NpgsqlCommand cmd = new("INSERT INTO deck_cards (deck_id, card_instance_id) VALUES (@deck_id, @card_instance_id);");
                    cmd.Parameters.AddWithValue("deck_id", deck.ID);
                    cmd.Parameters.AddWithValue("card_instance_id", deck.Cards[i].ID);
                    commands.Add(new(cmd, 1));
                }

                return _db.ExecuteNonQueryTransaction(commands);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool Update(Deck deck)
        {
            try
            {
                List<TransactionObject> commands = new();

                bool mainDeck;

                if (!deck.MainDeck)
                {
                    // Check if it is the first deck of the user => make it the main deck
                    mainDeck = GetUserDecks(deck.UserID).Count == 0;
                }
                else
                {
                    // Get old main deck
                    NpgsqlCommand cmdS = new("SELECT * FROM decks, user_decks WHERE user_id=@user_id AND main_deck=true;");
                    cmdS.Parameters.AddWithValue("user_id", deck.UserID);

                    if (Guid.TryParse(_db.SelectSingle(cmdS)?["id"]?.ToString() ?? "", out Guid oldMainDeckId))
                    {
                        // Uncheck old main deck
                        NpgsqlCommand cmd = new("UPDATE user_decks SET main_deck=False WHERE main_deck=True AND user_id=@user_id;");
                        cmd.Parameters.AddWithValue("user_id", deck.UserID);
                        commands.Add(new(cmd, 1));
                    }

                    // Set this deck as new main deck
                    mainDeck = true;
                }

                // Update main deck
                if (mainDeck)
                {
                    NpgsqlCommand cmd = new("UPDATE user_decks SET main_deck=True WHERE user_id=@user_id AND deck_id=@deck_id");
                    cmd.Parameters.AddWithValue("user_id", deck.UserID);
                    cmd.Parameters.AddWithValue("deck_id", deck.ID);
                    commands.Add(new(cmd, 1));
                }

                // Update deck information
                NpgsqlCommand cmd1 = new("UPDATE decks SET name=@name WHERE id=@deck_id;");
                cmd1.Parameters.AddWithValue("name", deck.Name);
                cmd1.Parameters.AddWithValue("deck_id", deck.ID);
                commands.Add(new(cmd1, 1));

                // Update cards
                // Unlink old cards
                NpgsqlCommand cmd2 = new("DELETE FROM deck_cards WHERE deck_id=@deck_id;");
                cmd2.Parameters.AddWithValue("deck_id", deck.ID);
                commands.Add(new(cmd2, Deck.DeckSize));

                // Link new cards
                for (int i = 0; i < deck.Cards.Count; i++)
                {
                    NpgsqlCommand cmd = new("INSERT INTO deck_cards (deck_id, card_instance_id) VALUES (@deck_id, @card_instance_id);");
                    cmd.Parameters.AddWithValue("deck_id", deck.ID);
                    cmd.Parameters.AddWithValue("card_instance_id", deck.Cards[i].ID);
                    commands.Add(new(cmd, 1));
                }

                return _db.ExecuteNonQueryTransaction(commands);
            }
            catch (Exception e)
            {
                _log.WriteLine(e.ToString());
                return false;
            }
        }

        public static Deck? ParseFromRow(OrderedDictionary deckInfo, OrderedDictionary[] cardRows, ILog _log)
        {
            try
            {
                List<CardInstance> cards = new();
                foreach (OrderedDictionary row in cardRows)
                {
                    CardInstance? card = CardInstanceRepository.ParseFromRow(row, _log);
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
