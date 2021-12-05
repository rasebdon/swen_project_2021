using Moq;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MTCG.Test.UnitTests.Repositories
{
    public class PackageRepositoryTest
    {
        private PackageRepository _repository;
        private Package _package;

        private TestLog _log;
        private Mock<IDatabase> _mockDb;

        [SetUp]
        public void SetUp()
        {
            _mockDb = new Mock<IDatabase>();
            _log = new TestLog();

            _package = new Package(
                Guid.NewGuid(), "testpackage", "blubb", 10, new List<Card>()
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
                });

            _repository = new PackageRepository(_mockDb.Object, _log);
        }

        [Test]
        public void InsertTest()
        {
            // Act
            bool insert = _repository.Insert(_package);

            // Assert
            Assert.IsFalse(insert); // No rows are affected in mocking
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQueryTransaction"), _mockDb.Invocations[0].Method); // Try insert (fails)
        }

        [Test]
        public void DeleteTest()
        {
            // Act
            bool delete = _repository.Delete(_package.ID);

            // Assert
            Assert.IsFalse(delete); // No rows are affected in mocking
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[0].Method); // Delete by id call
        }

        [Test]
        public void UpdateTest()
        {
            // Act & Assert
            Assert.Throws<NotImplementedException>(() => _repository.Update(_package));
        }

        [Test]
        public void ParseFromRowTest()
        {
            // Arrange
            OrderedDictionary packageRow = new();
            packageRow.Add("id", _package.ID);
            packageRow.Add("name", _package.Name);
            packageRow.Add("description", _package.Description);
            packageRow.Add("cost", _package.Cost);

            List<OrderedDictionary> cardRows = new();
            foreach (Card card in _package.Cards)
            {
                var row = new OrderedDictionary();
                row.Add("id", card.ID);
                row.Add("name", card.Name);
                row.Add("description", card.Description);
                row.Add("damage", card.Damage);
                row.Add("rarity", card.Rarity);
                row.Add("type", card.CardType);
                row.Add("race", card.Race);
                row.Add("element", card.Element);
                cardRows.Add(row);
            }

            // Act
            Package? package = PackageRepository.ParseFromRow(packageRow, cardRows.ToArray(), _log);

            // Assert
            Assert.NotNull(package);
            Assert.AreEqual(_package, package);
        }

        [Test]
        public void GetByIdTest()
        {
            // Act
            Package? package = _repository.GetById(Guid.Empty);

            // Assert
            Assert.IsNull(package); // No user in database
            Assert.AreEqual(2, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("SelectSingle"), _mockDb.Invocations[0].Method); // Select by id call
            Assert.AreEqual(typeof(IDatabase).GetMethod("Select"), _mockDb.Invocations[1].Method); // Select cards by id call
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
