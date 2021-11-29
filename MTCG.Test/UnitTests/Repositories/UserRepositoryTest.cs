using Moq;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using NUnit.Framework;
using System;
using System.Collections.Specialized;

namespace MTCG.Test.UnitTests.Repositories
{
    public class UserRepositoryTest
    {
        private UserRepository _repository;
        private User _user;

        private TestLog _log;
        private Mock<IDatabase> _mockDb;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _user = new User(Guid.NewGuid(), "test", "hash", 50, 50, 50);
            _mockDb = new Mock<IDatabase>();
            _log = new TestLog();
            _repository = new UserRepository(_mockDb.Object, _log);
        }

        [Test]
        public void InsertTest()
        {
            // Act
            bool insert = _repository.Insert(_user);

            // Assert
            Assert.IsFalse(insert); // No rows are affected in mocking
            Assert.AreEqual(2, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("SelectSingle"), _mockDb.Invocations[0].Method);    // Username check
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[1].Method); // Inserting
        }

        [Test]
        public void DeleteTest()
        {
            // Act
            bool delete = _repository.Delete(_user);

            // Assert
            Assert.IsFalse(delete); // No rows are affected in mocking
            Assert.AreEqual(2, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[0].Method); // Delete by id call
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[1].Method); // Delete by username call
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
            bool update = _repository.Update(_user, _user);

            // Assert
            Assert.IsFalse(update);
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("ExecuteNonQuery"), _mockDb.Invocations[0].Method); // Update call
        }


        [Test]
        public void TryUpdateWithNull()
        {
            // Act
            bool update1 = _repository.Update(_user, null);
            bool update2 = _repository.Update(null, _user);
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
            row.Add("id", _user.ID);
            row.Add("username", _user.Username);
            row.Add("hash", _user.Hash);
            row.Add("elo", _user.ELO);
            row.Add("played_games", _user.PlayedGames);
            row.Add("is_admin", _user.IsAdmin);
            row.Add("coins", _user.Coins);

            // Act
            User? user = UserRepository.ParseFromRow(row, _log);

            // Assert
            Assert.NotNull(user);
            Assert.AreEqual(_user, user);
        }

        [Test]
        public void GetByIdTest()
        {
            // Act
            User? user = _repository.GetById(Guid.Empty);

            // Assert
            Assert.IsNull(user); // No user in database
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("SelectSingle"), _mockDb.Invocations[0].Method); // Select by id call
        }

        [Test]
        public void GetByUsernameTest()
        {
            // Act
            User? user = _repository.GetByUsername("");

            // Assert
            Assert.IsNull(user); // No user in database
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("SelectSingle"), _mockDb.Invocations[0].Method); // Select by username call
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
