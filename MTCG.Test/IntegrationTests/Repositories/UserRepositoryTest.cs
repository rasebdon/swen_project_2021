using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Specialized;

namespace MTCG.Test.IntegrationTests.Repositories
{
    public class UserRepositoryTest
    {
        private UserRepository _repository;
        private User _user;

        private TestLog _log;
        private Database _db;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _user = new User(Guid.NewGuid(), "test", "hash", 50, 50, 50);
            _log = new TestLog();

            if (_db == null)
            {
                _db = new Database(
                    new DatabaseConfiguration("localhost", "mockdb", "mocking", "1234", 15000),
                    _log);
            }
            _db.OpenConnection();

            _repository = new UserRepository(_db, _log);
        }

        [Test]
        public void InsertTest()
        {
            // Arrange
            NpgsqlCommand cmd = new("SELECT * FROM users;");

            // Act
            // Test insert
            bool insert = _repository.Insert(_user);

            // Check if entry was inserted
            OrderedDictionary[] rows = _db.Select(cmd);
            User? inserted = UserRepository.ParseFromRow(rows[0], _log);

            // Delete entry
            cmd = new($"DELETE FROM users WHERE id='{_user.ID}';");
            _db.ExecuteNonQuery(cmd);

            // Assert
            Assert.IsNotNull(inserted);
            Assert.IsTrue(insert);              // Check if insert was successful
            Assert.AreEqual(1, rows?.Length);    // Check if row is really there
            Assert.AreEqual(_user.ID, inserted?.ID);    // Check if row is really there
        }

        [Test]
        public void TryInsertDuplicate()
        {
            // Arrange
            NpgsqlCommand cmd = new(
                @$"INSERT INTO users (id, username, hash, coins, elo, played_games, admin) 
                VALUES ('{_user.ID}', '{_user.Username}', '{_user.Hash}', {_user.Coins}, {_user.ELO}, {_user.PlayedGames}, {_user.IsAdmin});");

            // Act
            _db.ExecuteNonQuery(cmd);
            // Test insert
            bool insert = _repository.Insert(_user);

            // Delete entry
            cmd = new($"DELETE FROM users WHERE id='{_user.ID}';");
            _db.ExecuteNonQuery(cmd);

            Assert.IsFalse(insert);
        }

        [Test]
        public void DeleteTest()
        {
            // Arrange
            NpgsqlCommand cmd = new(
                @$"INSERT INTO users (id, username, hash, coins, elo, played_games, admin) 
                VALUES ('{_user.ID}', '{_user.Username}', '{_user.Hash}', {_user.Coins}, {_user.ELO}, {_user.PlayedGames}, {_user.IsAdmin});");

            // Act
            _db.ExecuteNonQuery(cmd);
            bool delete = _repository.Delete(_user.ID);

            // Check if row is not in db anymore
            cmd = new("SELECT * FROM users;");
            OrderedDictionary[] rows = _db.Select(cmd);

            // Assert
            Assert.IsTrue(delete);
            Assert.AreEqual(0, rows.Length);
        }

        [Test]
        public void UpdateTest()
        {
            // Arrange
            // Insert user
            NpgsqlCommand cmd = new(
                @$"INSERT INTO users (id, username, hash, coins, elo, played_games, admin) 
                VALUES ('{_user.ID}', '{_user.Username}', '{_user.Hash}', {_user.Coins}, {_user.ELO}, {_user.PlayedGames}, {_user.IsAdmin});");
            _db.ExecuteNonQuery(cmd);

            // Act
            _user.Coins = 999999;
            bool update = _repository.Update(_user, _user);

            // Select user
            cmd = new($"SELECT * FROM users WHERE id='{_user.ID}';");
            User? updated = UserRepository.ParseFromRow(_db.Select(cmd)[0], _log);

            // Delete entry
            cmd = new($"DELETE FROM users WHERE id='{_user.ID}';");
            _db.ExecuteNonQuery(cmd);

            // Assert
            Assert.IsNotNull(updated);
            Assert.IsTrue(update);
            Assert.AreEqual(_user.Coins, updated?.Coins);
        }

        [Test]
        public void GetByIdTest()
        {
            // Arrange
            // Insert user
            NpgsqlCommand cmd = new(
                @$"INSERT INTO users (id, username, hash, coins, elo, played_games, admin) 
                VALUES ('{_user.ID}', '{_user.Username}', '{_user.Hash}', {_user.Coins}, {_user.ELO}, {_user.PlayedGames}, {_user.IsAdmin});");
            _db.ExecuteNonQuery(cmd);

            // Act
            // Select user
            User? selected = _repository.GetById(_user.ID);

            // Delete entry
            cmd = new($"DELETE FROM users WHERE id='{_user.ID}';");
            _db.ExecuteNonQuery(cmd);

            // Assert
            Assert.NotNull(selected);
            Assert.AreEqual(_user, selected);
        }

        [Test]
        public void GetByUsernameTest()
        {
            // Arrange
            // Insert user
            NpgsqlCommand cmd = new(
                @$"INSERT INTO users (id, username, hash, coins, elo, played_games, admin) 
                VALUES ('{_user.ID}', '{_user.Username}', '{_user.Hash}', {_user.Coins}, {_user.ELO}, {_user.PlayedGames}, {_user.IsAdmin});");
            _db.ExecuteNonQuery(cmd);

            // Act
            // Select user
            User? selected = _repository.GetByUsername(_user.Username);

            // Delete entry
            cmd = new($"DELETE FROM users WHERE id='{_user.ID}';");
            _db.ExecuteNonQuery(cmd);

            // Assert
            Assert.NotNull(selected);
            Assert.AreEqual(_user, selected);
        }
    }
}
