using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTCG.Models;
using MTCG.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCGUnitTests.DatabaseTests
{
    [TestClass]
    public class CardTests
    {
        [TestMethod]
        public void InsertDeleteMonsterCard()
        {
            Card card = new MonsterCard("Monster", "This is a monster", 32, Element.Fire, Rarity.Legendary, Race.Beast);

            try
            {
                CardController.Instance.Insert(card);

                Assert.AreEqual(card, CardController.Instance.Select(card.ID));
            }
            finally
            {
                CardController.Instance.Delete(card);
            }
        }

        [TestMethod]
        public void InsertDeleteSpellCard()
        {
            Card card = new SpellCard("Spell", "This is a spell", 16, Element.Water, Rarity.Rare);

            try
            {
                CardController.Instance.Insert(card);
                Card insertedCard = CardController.Instance.Select(card.ID);

                Assert.AreEqual(card, insertedCard);
            }
            finally
            {
                CardController.Instance.Delete(card);
            }
        }

        [TestMethod]
        public void InsertDeleteCardInstance()
        {
            Card card = new MonsterCard("Monster", "This is a monster", 32, Element.Fire, Rarity.Legendary, Race.Beast);
            CardInstance instance = new(card);

            try
            {
                CardController.Instance.Insert(card);
                CardInstanceController.Instance.Insert(instance);

                CardInstance inserted = CardInstanceController.Instance.Select(instance.ID);

                Assert.AreEqual(instance.ID, inserted.ID);
                Assert.AreEqual(instance.CardID, inserted.CardID);
            }
            finally
            {
                CardInstanceController.Instance.Delete(instance);
                CardController.Instance.Delete(card);
            }
        }
    }
}
