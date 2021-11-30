using Moq;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using NUnit.Framework;
using System;
using System.Collections.Specialized;

namespace MTCG.Test.UnitTests.Repositories
{
    public class DeckRepositoryTest
    {
        private DeckRepository _repository;
        private Deck _deck;

        private TestLog _log;
        private Mock<IDatabase> _mockDb;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _mockDb = new Mock<IDatabase>();
            _log = new TestLog();

            _deck = new Deck(
                Guid.NewGuid(), "New Deck", Guid.NewGuid(), true, new CardInstance[2]
                {
                new CardInstance(Guid.NewGuid(), Guid.NewGuid(), "monster", "Mblubbdescr", 10, CardType.Monster, Element.Fire, Race.Undead, Rarity.Rare),
                new CardInstance(Guid.NewGuid(), Guid.NewGuid(), "spell", "Sblubbdescr", 5, CardType.Spell, Element.Fire, Race.None, Rarity.Rare)
                }
            );

            _repository = new DeckRepository(_mockDb.Object, _log);
        }

        [Test]
        public void InsertMainDeckTest()
        {
            // Arrange
            _deck.MainDeck = true;

            // Act
            bool insert = _repository.Insert(_deck);

            // Assert
            Assert.IsFalse(insert); // No rows are affected in mocking
            Assert.AreEqual(2, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("SelectSingle"), _mockDb.Invocations[0].Method); // Try find old main deck -> throws
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[1].Method); // Delete maybe inserted deck call
        }

        [Test]
        public void InsertNormalDeckTest()
        {
            // Arrange
            _deck.MainDeck = false;

            // Act
            bool insert = _repository.Insert(_deck);

            // Assert
            Assert.IsFalse(insert); // No rows are affected in mocking
            Assert.AreEqual(6, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("Select"), _mockDb.Invocations[0].Method); // Try find old main deck
            for (int i = 1; i < 6; i++)
            {
                Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[i].Method);
            }
        }


        [Test]
        public void DeleteTest()
        {
            // Act
            bool delete = _repository.Delete(_deck.ID);

            // Assert
            Assert.IsFalse(delete); // No rows are affected in mocking
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[0].Method); // Delete by id call
        }

        [Test]
        public void UpdateMainDeckTest()
        {
            // Arrange
            _deck.MainDeck = true;

            // Act
            bool update = _repository.Update(_deck, _deck);

            // Assert
            Assert.IsTrue(update);
            Assert.AreEqual(5, _mockDb.Invocations.Count);
        }

        [Test]
        public void UpdateNormalDeckTest()
        {
            // Arrange
            _deck.MainDeck = false;

            // Act
            bool update = _repository.Update(_deck, _deck);

            // Assert
            Assert.IsTrue(update);
            Assert.AreEqual(5, _mockDb.Invocations.Count);
        }

        [Test]
        public void ParseFromRowTest()
        {
            // Arrange
            OrderedDictionary deckRow = new();
            deckRow.Add("id", _deck.ID);
            deckRow.Add("name", _deck.Name);
            deckRow.Add("user_id", _deck.UserID);
            deckRow.Add("main_deck", _deck.MainDeck);

            OrderedDictionary[] cardRows = new OrderedDictionary[_deck.Cards.Count];
            for (int i = 0; i < _deck.Cards.Count; i++)
            {
                cardRows[i] = new OrderedDictionary();
                cardRows[i].Add("id", _deck.Cards[i].ID);
                cardRows[i].Add("card_id", _deck.Cards[i].CardID);
                cardRows[i].Add("name", _deck.Cards[i].Name);
                cardRows[i].Add("description", _deck.Cards[i].Description);
                cardRows[i].Add("damage", _deck.Cards[i].Damage);
                cardRows[i].Add("rarity", _deck.Cards[i].Rarity);
                cardRows[i].Add("type", _deck.Cards[i].CardType);
                cardRows[i].Add("race", _deck.Cards[i].Race);
                cardRows[i].Add("element", _deck.Cards[i].Element);
            }

            // Act
            Deck? deck = DeckRepository.ParseFromRow(deckRow, cardRows, _log);

            // Assert
            Assert.NotNull(deck);
            Assert.AreEqual(_deck, deck);
        }

        [Test]
        public void GetByIdTest()
        {
            // Act
            Deck? deck = _repository.GetById(Guid.Empty);

            // Assert
            Assert.IsNull(deck); // No user in database
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
