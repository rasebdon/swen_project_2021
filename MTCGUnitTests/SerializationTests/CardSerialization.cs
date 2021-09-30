using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
using MTCG.Serialization;
using Newtonsoft.Json;

namespace MTCGUnitTests.SerializationTests
{
    [TestClass]
    public class CardSerialization
    {
        [TestMethod]
        public void SerializeCard()
        {
            var card = DummyMonsterCard();
            string json = card.ToJson();

            var converter = new CardConverter();

            Card serialized = JsonConvert.DeserializeObject<Card>(json, converter);

            Assert.AreEqual(card.GetType(), serialized.GetType());
            Assert.AreEqual(card.Race, (serialized as MonsterCard).Race);
        }
        
        [TestMethod]
        public void SerializeCardList()
        {
            var cards = new List<Card>() 
            { 
                DummyMonsterCard(),
                DummySpellCard(),
                DummyMonsterCard()
            };
            string json = JsonConvert.SerializeObject(cards);

            var converter = new CardConverter();

            List<Card> serialized = JsonConvert.DeserializeObject<List<Card>>(json, converter);

            Assert.AreEqual(cards.GetType(), serialized.GetType());
            Assert.AreEqual(cards.Count, serialized.Count);
            Assert.AreEqual(typeof(MonsterCard), serialized[0].GetType());
            Assert.AreEqual(typeof(SpellCard), serialized[1].GetType());
            Assert.AreEqual(typeof(MonsterCard), serialized[2].GetType());
        }

        static MonsterCard DummyMonsterCard()
        {
            return new MonsterCard(
                    "Lazy Peon",
                    "No work...",
                    3,
                    Element.Normal,
                    Rarity.Common,
                    Race.Orc);
        }

        static SpellCard DummySpellCard()
        {
            return new SpellCard(
                    "Flame Lance",
                    "A fiery lance that not many mages are able to cast",
                    5,
                    Element.Fire,
                    Rarity.Rare);
        }
    }
}
