using Moq;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MTCG.Test.UnitTests.Repositories
{
    public class StackRepositoryTest
    {
        private StackRepository _repository;
        private List<CardInstance> _stack;

        private TestLog _log;
        private Mock<IDatabase> _mockDb;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _stack = new List<CardInstance>()
            {
                new CardInstance(Guid.NewGuid(), Guid.NewGuid(), "monster", "Mblubbdescr", 10, CardType.Monster, Element.Fire, Race.Undead, Rarity.Rare),
                new CardInstance(Guid.NewGuid(), Guid.NewGuid(), "spell", "Sblubbdescr", 5, CardType.Spell, Element.Fire, Race.None, Rarity.Rare),
            };
            _mockDb = new Mock<IDatabase>();
            _log = new TestLog();
            _repository = new StackRepository(_mockDb.Object, _log);
        }

        [Test]
        public void InsertNotImplementedTest()
        {
            // Act & Assert
            Assert.Throws<NotImplementedException>(() => _repository.Delete(Guid.Empty));
        }

        [Test]
        public void DeleteNotImplementedTest()
        {
            // Act & Assert
            Assert.Throws<NotImplementedException>(() => _repository.Delete(Guid.Empty));
        }

        [Test]
        public void UpdateNotImplementedTest()
        {
            // Act & Assert
            Assert.Throws<NotImplementedException>(() => _repository.Update(_stack, _stack));
        }

        [Test]
        public void GetByIdTest()
        {
            // Act
            List<CardInstance>? stack = _repository.GetById(Guid.Empty);

            // Assert
            Assert.IsNotNull(stack);
            Assert.IsEmpty(stack); // No stack in mocked database
            Assert.AreEqual(1, _mockDb.Invocations.Count);
            Assert.AreEqual(typeof(IDatabase).GetMethod("Select"), _mockDb.Invocations[0].Method); // Select by id call
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
