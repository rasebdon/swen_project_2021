using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Specialized;

namespace MTCG.Test.IntegrationTests.Repositories
{
    public class CardRepositoryTest
    {
        private CardRepository _repository;
        private Card _card;

        private TestLog _log;
        private Database _db;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _card = new Card(
                Guid.NewGuid(),
                "Spell Name",
                "Description!!!",
                5,
                CardType.Spell,
                Element.Fire,
                Race.None,
                Rarity.Rare);

            _log = new TestLog();

            if (_db == null)
            {
                _db = new Database(
                    new DatabaseConfiguration("localhost", "mockdb", "mocking", "1234", 15000),
                    _log);
            }
            _db.OpenConnection();

            _repository = new CardRepository(_db, _log);
        }

        [Test]
        public void InsertTest()
        {
            // Arrange
            NpgsqlCommand cmd = new("SELECT * FROM cards;");

            // Act
            // Test insert
            bool insert = _repository.Insert(_card);

            // Check if entry was inserted
            OrderedDictionary[] rows = _db.Select(cmd);
            Card? inserted = CardRepository.ParseFromRow(rows[0], _log);

            // Delete entry
            cmd = new($"DELETE FROM cards WHERE id='{_card.ID}';");
            _db.ExecuteNonQuery(cmd);

            // Assert
            Assert.IsNotNull(inserted);
            Assert.IsTrue(insert);              // Check if insert was successful
            Assert.AreEqual(1, rows?.Length);    // Check if row is really there
            Assert.AreEqual(_card.ID, inserted?.ID);    // Check if row is really there
        }

        [Test]
        public void TryInsertDuplicate()
        {
            // Arrange
            NpgsqlCommand cmd = new(
                @$"INSERT INTO cards (id, name, description, damage, type, element, race, rarity) 
                VALUES ('{_card.ID}', '{_card.Name}', '{_card.Description}', {_card.Damage},
                {(int)_card.CardType}, {(int)_card.Element}, {(int)_card.Race}, {(int)_card.Rarity});");

            // Act
            _db.ExecuteNonQuery(cmd);
            // Test insert
            bool insert = _repository.Insert(_card);

            // Delete entry
            cmd = new($"DELETE FROM cards WHERE id='{_card.ID}';");
            _db.ExecuteNonQuery(cmd);

            Assert.IsFalse(insert);
        }

        [Test]
        public void DeleteTest()
        {
            // Arrange
            NpgsqlCommand cmd = new(
                @$"INSERT INTO cards (id, name, description, damage, type, element, race, rarity) 
                VALUES ('{_card.ID}', '{_card.Name}', '{_card.Description}', {_card.Damage},
                {(int)_card.CardType}, {(int)_card.Element}, {(int)_card.Race}, {(int)_card.Rarity});");

            // Act
            _db.ExecuteNonQuery(cmd);
            bool delete = _repository.Delete(_card.ID);

            // Check if row is not in db anymore
            cmd = new("SELECT * FROM cards;");
            OrderedDictionary[] rows = _db.Select(cmd);

            // Assert
            Assert.IsTrue(delete);
            Assert.AreEqual(0, rows.Length);
        }

        [Test]
        public void UpdateTest()
        {
            // Arrange
            NpgsqlCommand cmd = new(
                @$"INSERT INTO cards (id, name, description, damage, type, element, race, rarity) 
                VALUES ('{_card.ID}', '{_card.Name}', '{_card.Description}', {_card.Damage},
                {(int)_card.CardType}, {(int)_card.Element}, {(int)_card.Race}, {(int)_card.Rarity});");

            // Act
            // Insert card
            _db.ExecuteNonQuery(cmd);

            // Update card
            _card.Damage = 999999;
            bool update = _repository.Update(_card);

            // Select card
            cmd = new($"SELECT * FROM cards WHERE id='{_card.ID}';");
            Card? updated = CardRepository.ParseFromRow(_db.Select(cmd)[0], _log);

            // Delete entry
            cmd = new($"DELETE FROM cards WHERE id='{_card.ID}';");
            _db.ExecuteNonQuery(cmd);

            // Assert
            Assert.IsNotNull(updated);
            Assert.IsTrue(update);
            Assert.AreEqual(_card.Damage, updated?.Damage);
        }

        [Test]
        public void GetByIdTest()
        {
            // Arrange
            NpgsqlCommand cmd = new(
                @$"INSERT INTO cards (id, name, description, damage, type, element, race, rarity) 
                VALUES ('{_card.ID}', '{_card.Name}', '{_card.Description}', {_card.Damage},
                {(int)_card.CardType}, {(int)_card.Element}, {(int)_card.Race}, {(int)_card.Rarity});");

            // Act
            // Insert cards
            _db.ExecuteNonQuery(cmd);

            // Select card
            Card? selected = _repository.GetById(_card.ID);

            // Delete entry
            cmd = new($"DELETE FROM cards WHERE id='{_card.ID}';");
            _db.ExecuteNonQuery(cmd);

            // Assert
            Assert.NotNull(selected);
            Assert.AreEqual(_card, selected);
        }
    }
}
