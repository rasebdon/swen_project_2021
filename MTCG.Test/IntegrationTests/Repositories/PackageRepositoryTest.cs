using Moq;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MTCG.Test.IntegrationTests.Repositories
{
    public class PackageRepositoryTest
    {
        private PackageRepository _repository;
        private Package _package;
        private Card[] _cards;

        private TestLog _log;
        private IDatabase _db;

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

            _cards = new Card[5]
            {
                new Card(Guid.NewGuid(), "Card1", "This is a card1!",
                10, CardType.Monster, Element.Normal, Race.Beast, Rarity.Rare),
                new Card(Guid.NewGuid(), "Card2", "This is a card2!",
                6, CardType.Monster, Element.Water, Race.Human, Rarity.Legendary),
                new Card(Guid.NewGuid(), "Card3", "This is a card3!",
                12, CardType.Monster, Element.Normal, Race.Mage, Rarity.Epic),
                new Card(Guid.NewGuid(), "Card4", "This is a card4!",
                18, CardType.Spell, Element.Water, Race.None, Rarity.Rare),
                new Card(Guid.NewGuid(), "Card5", "This is a card5!",
                32, CardType.Spell, Element.Fire, Race.None, Rarity.Common),
            };
            // Insert cards
            foreach (Card card in _cards)
            {
                _db.ExecuteNonQuery(new NpgsqlCommand(
                    @$"INSERT INTO cards (id, name, description, damage, type, element, race, rarity) 
                    VALUES ('{card.ID}', '{card.Name}', '{card.Description}', {card.Damage},
                    {(int)card.CardType}, {(int)card.Element}, {(int)card.Race}, {(int)card.Rarity});"));
            }

            _package = new Package(Guid.NewGuid(), "testpackage", "blubb", 10, _cards);

            _repository = new PackageRepository(_db, _log);
        }

        [Test]
        public void InsertTest()
        {
            // Act
            bool insert = _repository.Insert(_package);

            // Select inserted package
            var packageInfoRow = _db.SelectSingle(
                new NpgsqlCommand($"SELECT * FROM packages WHERE id='{_package.ID}';"));
            var packageCards = _db.Select(
                new NpgsqlCommand($"SELECT * FROM package_cards, cards WHERE package_id='{_package.ID}' AND cards.id=package_cards.card_id;"));

            Package? inserted = PackageRepository.ParseFromRow(
                packageInfoRow,
                packageCards,
                _log);

            // Assert
            Assert.IsTrue(insert);
            Assert.AreEqual(_package, inserted);
        }

        [Test]
        public void DeleteTest()
        {
            // Arrange
            // Insert package data
            NpgsqlCommand cmd = new(
                @$"INSERT INTO packages (id, name, description, cost) 
                    VALUES ('{_package.ID}', '{_package.Name}', '{_package.Description}', {_package.Cost});");
            _db.ExecuteNonQuery(cmd);
            // Link cards to package
            foreach (Card card in _package.Cards)
            {
                cmd = new($"INSERT INTO package_cards (package_id, card_id) VALUES ('{_package.ID}', '{card.ID}');");
                _db.ExecuteNonQuery(cmd);
            }

            // Act
            bool delete = _repository.Delete(_package.ID);

            // Select deleted package
            var packageInfoRow = _db.SelectSingle(
                new NpgsqlCommand($"SELECT * FROM packages WHERE id='{_package.ID}';"));
            var packageCards = _db.Select(
                new NpgsqlCommand($"SELECT * FROM package_cards, cards WHERE package_id='{_package.ID}' AND cards.id=package_cards.card_id;"));

            Package? inserted = PackageRepository.ParseFromRow(
                packageInfoRow,
                packageCards,
                _log);

            // Assert
            Assert.IsTrue(delete);
            Assert.AreEqual(0, packageCards.Length);
            Assert.AreEqual(0, packageInfoRow.Count);
            Assert.AreEqual(inserted, null);
        }

        [Test]
        public void UpdateNotImplementedTest()
        {
            // Act & Assert
            Assert.Throws<NotImplementedException>(() => _repository.Update(_package));
        }

        [Test]
        public void GetByIdTest()
        {
            // Arrange
            // Insert package data
            NpgsqlCommand cmd = new(
                @$"INSERT INTO packages (id, name, description, cost) 
                    VALUES ('{_package.ID}', '{_package.Name}', '{_package.Description}', {_package.Cost});");
            _db.ExecuteNonQuery(cmd);
            // Link cards to package
            foreach (Card card in _package.Cards)
            {
                cmd = new($"INSERT INTO package_cards (package_id, card_id) VALUES ('{_package.ID}', '{card.ID}');");
                _db.ExecuteNonQuery(cmd);
            }

            // Act
            Package? package = _repository.GetById(_package.ID);

            // Assert
            Assert.AreEqual(_package, package);
        }

        [TearDown]
        public void TearDown()
        {
            // Delete entries
            NpgsqlCommand cmd = new($"DELETE FROM packages; DELETE FROM cards;");
            _db.ExecuteNonQuery(cmd);
        }
    }
}
