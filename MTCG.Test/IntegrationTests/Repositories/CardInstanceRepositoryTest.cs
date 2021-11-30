using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Specialized;

namespace MTCG.Test.IntegrationTests.Repositories
{
    public class CardInstanceRepositoryTest
    {
        private CardInstanceRepository _repository;
        private CardInstance _cardInstance;
        private Card _card;

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
            _repository = new CardInstanceRepository(_db, _log);

            // Create cards
            _card = new Card(
                Guid.NewGuid(),
                "Spell Name",
                "Description!!!",
                5,
                CardType.Spell,
                Element.Fire,
                Race.None,
                Rarity.Rare);
            _cardInstance = new CardInstance(Guid.NewGuid(), _card);

            // Arrange
            // Insert card 1
            _db.ExecuteNonQuery(new NpgsqlCommand(
                @$"INSERT INTO cards (id, name, description, damage, type, element, race, rarity) 
                VALUES ('{_card.ID}', '{_card.Name}', '{_card.Description}', {_card.Damage},
                {(int)_card.CardType}, {(int)_card.Element}, {(int)_card.Race}, {(int)_card.Rarity});"));
            // Insert card instance
            _db.ExecuteNonQuery(new NpgsqlCommand(
                @$"INSERT INTO card_instances (id, card_id) 
                VALUES ('{_cardInstance.ID}', '{_card.ID}');"));
        }

        [Test]
        public void InsertTest()
        {
            // Act
            // Delete card instance from setup
            _db.ExecuteNonQuery(new NpgsqlCommand(@"DELETE FROM card_instances;"));

            // Test insert
            bool insert = _repository.Insert(_cardInstance);

            // Check if entry was inserted
            NpgsqlCommand cmd = new(
                @"SELECT card_instances.*, cards.type, cards.name, cards.description,
                cards.damage, cards.element, cards.rarity, cards.race  FROM card_instances, cards
                WHERE card_id=cards.id;");
            OrderedDictionary[] rows = _db.Select(cmd);
            CardInstance? inserted = CardInstanceRepository.ParseFromRow(rows[0], _log);

            // Assert
            Assert.IsNotNull(inserted);
            Assert.IsTrue(insert);              // Check if insert was successful
            Assert.AreEqual(1, rows?.Length);    // Check if row is really there
            Assert.AreEqual(_cardInstance, inserted);    // Check if row is really there
        }

        [Test]
        public void TryInsertDuplicate()
        {
            // Act
            // Test insert
            bool insert = _repository.Insert(_cardInstance);

            Assert.IsFalse(insert);
        }

        [Test]
        public void DeleteTest()
        {
            // Act
            bool delete = _repository.Delete(_cardInstance.ID);

            // Check if row is not in db anymore
            OrderedDictionary[] rows = _db.Select(new NpgsqlCommand("SELECT * FROM card_instances;"));

            // Assert
            Assert.IsTrue(delete);
            Assert.AreEqual(0, rows.Length);
        }

        [Test]
        public void UpdateTest()
        {
            // Act
            // Insert card 2
            Card card2 = new(Guid.NewGuid(), "New card!!!", "descr", 22, CardType.Spell, Element.Water, Race.None, Rarity.Legendary);
            _db.ExecuteNonQuery(new NpgsqlCommand(
                @$"INSERT INTO cards (id, name, description, damage, type, element, race, rarity) 
                VALUES ('{card2.ID}', '{card2.Name}', '{card2.Description}', {card2.Damage},
                {(int)card2.CardType}, {(int)card2.Element}, {(int)card2.Race}, {(int)card2.Rarity});"));

            // Act
            // Update card instance
            _cardInstance.CardID = card2.ID;
            bool update = _repository.Update(_cardInstance, _cardInstance);

            // Select card
            CardInstance? updated = CardInstanceRepository.ParseFromRow(
                _db.Select(new NpgsqlCommand(@"SELECT card_instances.*, cards.type, cards.name, cards.description,
                cards.damage, cards.element, cards.rarity, cards.race  FROM card_instances, cards
                WHERE card_id=cards.id;"))[0], _log);

            // Assert
            Assert.IsNotNull(updated);
            Assert.IsTrue(update);
            Assert.AreEqual(_cardInstance.CardID, updated?.CardID);
        }

        [Test]
        public void GetByIdTest()
        {
            // Act
            // Select card instance
            CardInstance? selected = _repository.GetById(_cardInstance.ID);

            // Assert
            Assert.NotNull(selected);
            Assert.AreEqual(_cardInstance, selected);
        }

        [TearDown]
        public void TearDown()
        {
            // Delete entry
            NpgsqlCommand cmd = new($"DELETE FROM cards; DELETE FROM card_instances;");
            _db.ExecuteNonQuery(cmd);
        }
    }
}
