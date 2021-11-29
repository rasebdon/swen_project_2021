using Moq;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using NUnit.Framework;
using System;
using System.Collections.Specialized;

namespace MTCG.Test.UnitTests.Repositories
{
    public class CardInstanceRepositoryTest
    {
        private CardInstanceRepository _repository;
        private CardInstance _card;

        private TestLog _log;
        private Mock<IDatabase> _mockDb;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _mockDb = new Mock<IDatabase>();
            _log = new TestLog();

            _card = new CardInstance(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Spell Name",
                "Description!!!",
                5,
                CardType.Spell,
                Element.Fire,
                Race.None,
                Rarity.Rare);

            _repository = new CardInstanceRepository(_mockDb.Object, _log);
        }

        [Test]
        public void InsertTest()
        {
            // Act
            bool insert = _repository.Insert(_card);

            // Assert
            Assert.IsFalse(insert); // No rows are affected in mocking
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[0].Method); // Inserting
        }

        [Test]
        public void DeleteTest()
        {
            // Act
            bool delete = _repository.Delete(_card);

            // Assert
            Assert.IsFalse(delete); // No rows are affected in mocking
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[0].Method); // Delete by id call
        }


        [Test]
        public void TryDeleteWithNullObject()
        {
            // Act
            bool delete = _repository.Delete(null);

            // Assert
            Assert.IsFalse(delete); // No rows are affected in mocking
            Assert.AreEqual(0, _mockDb.Invocations.Count);
        }


        [Test]
        public void UpdateTest()
        {
            // Act
            bool update = _repository.Update(_card, _card);

            // Assert
            Assert.IsFalse(update);
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[0].Method); // Update call
        }


        [Test]
        public void TryUpdateWithNull()
        {
            // Act
            bool update1 = _repository.Update(_card, null);
            bool update2 = _repository.Update(null, _card);
            bool update3 = _repository.Update(null, null);

            Assert.IsFalse(update1);
            Assert.IsFalse(update2);
            Assert.IsFalse(update3);
            Assert.AreEqual(0, _mockDb.Invocations.Count);
        }

        [Test]
        public void ParseFromRowTest()
        {
            // Arrange
            OrderedDictionary row = new();
            row.Add("id", _card.ID);
            row.Add("card_id", _card.ID);
            row.Add("name", _card.Name);
            row.Add("description", _card.Description);
            row.Add("damage", _card.Damage);
            row.Add("rarity", _card.Rarity);
            row.Add("card_type", _card.CardType);
            row.Add("race", _card.Race);
            row.Add("element", _card.Element);

            // Act
            Card? card = CardRepository.ParseFromRow(row, _log);

            // Assert
            Assert.NotNull(card);
            Assert.AreEqual(_card, card);
        }

        [Test]
        public void GetByIdTest()
        {
            // Act
            Card? card = _repository.GetById(Guid.Empty);

            // Assert
            Assert.IsNull(card); // No user in database
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("SelectSingle"), _mockDb.Invocations[0].Method); // Select by id call
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
