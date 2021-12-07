using Moq;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MTCG.Test.IntegrationTests.Repositories
{
    public class StackRepositoryTest
    {
        private StackRepository _repository;
        private User _user;
        private Card[] _cards;
        private CardInstance[] _stack;

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

            _repository = new StackRepository(_db, _log);
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
            _stack = new CardInstance[10]
            {
                new CardInstance(Guid.NewGuid(), _cards[0]),
                new CardInstance(Guid.NewGuid(), _cards[1]),
                new CardInstance(Guid.NewGuid(), _cards[0]),
                new CardInstance(Guid.NewGuid(), _cards[1]),
                new CardInstance(Guid.NewGuid(), _cards[1]),
                new CardInstance(Guid.NewGuid(), _cards[0]),
                new CardInstance(Guid.NewGuid(), _cards[1]),
                new CardInstance(Guid.NewGuid(), _cards[0]),
                new CardInstance(Guid.NewGuid(), _cards[0]),
                new CardInstance(Guid.NewGuid(), _cards[1]),
            };
            // Insert card instances
            foreach (CardInstance cardInstance in _stack)
            {
                _db.ExecuteNonQuery(new NpgsqlCommand(
                    @$"INSERT INTO card_instances (id, card_id) 
                    VALUES ('{cardInstance.ID}', '{cardInstance.CardID}');"));
            }

            // Create user
            _user = new(Guid.NewGuid(), "rasebdon", "hash", 20, 5, 25, "Im playing MTCG!", ":-)", 0);
            // Insert user
            _db.ExecuteNonQuery(new NpgsqlCommand(
                @$"INSERT INTO users (id, username, hash, coins, elo, played_games, admin) 
                VALUES ('{_user.ID}', '{_user.Username}', '{_user.Hash}', {_user.Coins}, {_user.ELO}, {_user.PlayedGames}, {_user.IsAdmin});"));

            // Connect cards to user
            foreach (CardInstance cardInstance in _stack)
            {
                _db.ExecuteNonQuery(new NpgsqlCommand(
                    @$"INSERT INTO user_cards (card_instance_id, user_id) 
                    VALUES ('{cardInstance.ID}', '{_user.ID}');"));
            }
        }

        [Test]
        public void GetByIdTest()
        {
            // Act
            List<CardInstance>? stack = _repository.GetById(_user.ID);

            // Assert
            Assert.IsNotNull(stack);
            Assert.AreEqual(_stack.Length, stack?.Count);
            for (int i = 0; i < _stack.Length; i++)
            {
                Assert.IsTrue(stack?.Contains(_stack[i]));
            }
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
