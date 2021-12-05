using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Specialized;

namespace MTCG.Test.IntegrationTests.Repositories
{
    public class DeckRepositoryTest
    {
        private DeckRepository _repository;
        private User _user;
        private Deck _deck;
        private Card[] _cards;
        private CardInstance[] _cardInstances;

        private TestLog _log;
        private Database _db;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _log = new TestLog();
            if (_db == null)
            {
                _db = new Database(
                    new DatabaseConfiguration("localhost", "mockdb", "mocking", "1234", 15000),
                    _log);
            }
            _db.OpenConnection();

            _repository = new DeckRepository(_db, _log);

            // Create cards
            _cards = new Card[2]
            {
                new Card(Guid.NewGuid(), "monster", "Mblubbdescr", 10, CardType.Monster, Element.Fire, Race.Undead, Rarity.Rare),
                new Card(Guid.NewGuid(), "spell", "Sblubbdescr", 5, CardType.Spell, Element.Fire, Race.None, Rarity.Rare),
            };
            // Insert cards
            foreach (Card card in _cards)
            {
                _db.ExecuteNonQuery(new NpgsqlCommand(
                    @$"INSERT INTO cards (id, name, description, damage, type, element, race, rarity) 
                    VALUES ('{card.ID}', '{card.Name}', '{card.Description}', {card.Damage},
                    {(int)card.CardType}, {(int)card.Element}, {(int)card.Race}, {(int)card.Rarity});"));
            }

            // Create instances
            _cardInstances = new CardInstance[Deck.DeckSize]
            {
                new CardInstance(Guid.NewGuid(), _cards[0]),
                new CardInstance(Guid.NewGuid(), _cards[1]),
                new CardInstance(Guid.NewGuid(), _cards[0]),
                new CardInstance(Guid.NewGuid(), _cards[1]),
            };
            // Insert card instances
            foreach (CardInstance cardInstance in _cardInstances)
            {
                _db.ExecuteNonQuery(new NpgsqlCommand(
                    @$"INSERT INTO card_instances (id, card_id) 
                    VALUES ('{cardInstance.ID}', '{cardInstance.CardID}');"));
            }

            // Create user
            _user = new(Guid.NewGuid(), "rasebdon", "hash", 20, 5, 25);
            // Insert user
            _db.ExecuteNonQuery(new NpgsqlCommand(
                @$"INSERT INTO users (id, username, hash, coins, elo, played_games, admin) 
                VALUES ('{_user.ID}', '{_user.Username}', '{_user.Hash}', {_user.Coins}, {_user.ELO}, {_user.PlayedGames}, {_user.IsAdmin});"));

            // Connect cards to user
            foreach (CardInstance cardInstance in _cardInstances)
            {
                _db.ExecuteNonQuery(new NpgsqlCommand(
                    @$"INSERT INTO user_cards (card_instance_id, user_id) 
                    VALUES ('{cardInstance.ID}', '{_user.ID}');"));
            }

            // Create deck
            _deck = new Deck(Guid.NewGuid(), "New Deck", _user.ID, true, _cardInstances);

        }
        [Test]
        [TestCase(true, TestName = "InsertMainDeckIntegrationTest")]
        [TestCase(false, TestName = "InsertNormalDeckIntegrationTest")]
        public void InsertTest(bool mainDeck)
        {
            // Arrange
            _deck.MainDeck = mainDeck;

            // Act
            bool insert = _repository.Insert(_deck);

            // Retrieve inserted deck
            NpgsqlCommand cmd = new($@"SELECT * FROM decks, user_decks WHERE id='{_deck.ID}' AND deck_id=id;");
            var deckRow = _db.SelectSingle(cmd);
            cmd = new(
                @$"SELECT card_instances.*, cards.type, cards.name, cards.description,
                cards.damage, cards.element, cards.rarity, cards.race  FROM card_instances, cards, decks
                WHERE card_id=cards.id AND decks.id='{_deck.ID}';");
            var cardRows = _db.Select(cmd);
            Deck? result = DeckRepository.ParseFromRow(deckRow, cardRows, _log);

            // Assert
            Assert.IsTrue(insert);
            Assert.IsNotNull(result);
            if (!mainDeck)
            {
                Assert.IsTrue(result?.MainDeck);
                if(result != null)
                    result.MainDeck = false;
            }
            Assert.AreEqual(_deck, result);
        }

        [Test]
        public void DeleteTest()
        {
            // Arrange
            // Insert deck metadata
            NpgsqlCommand cmd = new(
                $"INSERT INTO decks (id, name) VALUES ('{_deck.ID}', '{_deck.Name}');");
            cmd.Parameters.AddWithValue("id", _deck.ID);
            cmd.Parameters.AddWithValue("name", _deck.Name);
            _db.ExecuteNonQuery(cmd);

            // Insert user link
            cmd = new(
                @$"INSERT INTO user_decks (deck_id, user_id, main_deck)
                VALUES ('{_deck.ID}', '{_deck.UserID}', '{_deck.MainDeck}');");
            _db.ExecuteNonQuery(cmd);

            // Insert card links
            for (int i = 0; i < _deck.Cards.Count; i++)
            {
                cmd = new($"INSERT INTO deck_cards (deck_id, card_instance_id) VALUES ('{_deck.ID}', '{_deck.Cards[i].ID}');");
                _db.ExecuteNonQuery(cmd);
            }

            // Act
            bool delete = _repository.Delete(_deck.ID);

            // Try to retrieve deleted deck
            cmd = new($@"SELECT * FROM decks, user_decks WHERE id='{_deck.ID}' AND deck_id=id;");
            var deckRow = _db.SelectSingle(cmd);
            cmd = new(
                @$"SELECT card_instances.*, cards.type, cards.name, cards.description,
                cards.damage, cards.element, cards.rarity, cards.race  FROM card_instances, cards, decks
                WHERE card_id=cards.id AND decks.id='{_deck.ID}';");
            var cardRows = _db.Select(cmd);
            Deck? result = DeckRepository.ParseFromRow(deckRow, cardRows, _log);

            // Assert
            Assert.IsTrue(delete);
            Assert.IsNull(result);
        }

        [Test]
        [TestCase(true, TestName = "UpdateMainDeckIntegrationTest")]
        [TestCase(false, TestName = "UpdateNormalDeckIntegrationTest")]
        public void UpdateMainDeckTest(bool mainDeck)
        {
            // Arrange
            _deck.MainDeck = mainDeck;
            // Insert deck metadata
            NpgsqlCommand cmd = new(
                $"INSERT INTO decks (id, name) VALUES ('{_deck.ID}', '{_deck.Name}');");
            cmd.Parameters.AddWithValue("id", _deck.ID);
            cmd.Parameters.AddWithValue("name", _deck.Name);
            _db.ExecuteNonQuery(cmd);

            // Insert user link
            cmd = new(
                @$"INSERT INTO user_decks (deck_id, user_id, main_deck)
                VALUES ('{_deck.ID}', '{_deck.UserID}', '{_deck.MainDeck}');");
            _db.ExecuteNonQuery(cmd);

            // Insert card links
            for (int i = 0; i < _deck.Cards.Count; i++)
            {
                cmd = new($"INSERT INTO deck_cards (deck_id, card_instance_id) VALUES ('{_deck.ID}', '{_deck.Cards[i].ID}');");
                _db.ExecuteNonQuery(cmd);
            }

            // Change deck name
            _deck.Name = "Blubb!!!";

            // Act
            bool update = _repository.Update(_deck);

            // Select updated
            cmd = new($@"SELECT * FROM decks, user_decks WHERE id='{_deck.ID}' AND deck_id=id;");
            var deckRow = _db.SelectSingle(cmd);
            cmd = new(
                @$"SELECT card_instances.*, cards.type, cards.name, cards.description,
                cards.damage, cards.element, cards.rarity, cards.race  FROM card_instances, cards, decks
                WHERE card_id=cards.id AND decks.id='{_deck.ID}';");
            var cardRows = _db.Select(cmd);
            Deck? updated = DeckRepository.ParseFromRow(deckRow, cardRows, _log);

            // Assert
            Assert.IsTrue(update);
            Assert.AreEqual(_deck, updated);
        }

        [Test]
        public void GetByIdTest()
        {
            // Arrange
            // Insert deck metadata
            NpgsqlCommand cmd = new(
                $"INSERT INTO decks (id, name) VALUES ('{_deck.ID}', '{_deck.Name}');");
            cmd.Parameters.AddWithValue("id", _deck.ID);
            cmd.Parameters.AddWithValue("name", _deck.Name);
            _db.ExecuteNonQuery(cmd);

            // Insert user link
            cmd = new(
                @$"INSERT INTO user_decks (deck_id, user_id, main_deck)
                VALUES ('{_deck.ID}', '{_deck.UserID}', '{_deck.MainDeck}');");
            _db.ExecuteNonQuery(cmd);

            // Insert card links
            for (int i = 0; i < _deck.Cards.Count; i++)
            {
                cmd = new($"INSERT INTO deck_cards (deck_id, card_instance_id) VALUES ('{_deck.ID}', '{_deck.Cards[i].ID}');");
                _db.ExecuteNonQuery(cmd);
            }

            // Act
            Deck? selected = _repository.GetById(_deck.ID);

            // Assert
            Assert.AreEqual(_deck, selected);
        }

        [TearDown]
        public void TearDown()
        {
            // Delete entries
            NpgsqlCommand cmd = new($"DELETE FROM decks; DELETE FROM cards; DELETE FROM card_instances; DELETE FROM users;");
            _db.ExecuteNonQuery(cmd);
        }
    }
}
