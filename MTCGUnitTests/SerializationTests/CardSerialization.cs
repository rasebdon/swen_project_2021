//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using MTCG.Models;
//using Newtonsoft.Json;
//using System.Collections.Generic;

//namespace MTCGUnitTests.SerializationTests
//{
//    [TestClass]
//    public class CardSerialization
//    {
//        [TestMethod]
//        public void SerializeCard()
//        {
//            var card = DummyMonsterCard();
            
//            string json = card.ToJson();
//            Card serialized = JsonConvert.DeserializeObject<Card>(json);

//            Assert.AreEqual(card.GetType(), serialized.GetType());
//            Assert.AreEqual(card.Race, serialized.Race);
//            Assert.AreEqual(card.CardType, serialized.CardType);
//        }

//        [TestMethod]
//        public void SerializeCardList()
//        {
//            var cards = new List<Card>()
//            {
//                DummyMonsterCard(),
//                DummySpellCard(),
//                DummyMonsterCard()
//            };
//            string json = JsonConvert.SerializeObject(cards);
//            List<Card> serialized = JsonConvert.DeserializeObject<List<Card>>(json);

//            Assert.AreEqual(cards.GetType(), serialized.GetType());
//            Assert.AreEqual(cards.Count, serialized.Count);
//            Assert.AreEqual(CardType.Monster, serialized[0].CardType);
//            Assert.AreEqual(CardType.Spell, serialized[1].CardType);
//            Assert.AreEqual(CardType.Monster, serialized[2].CardType);
//        }

//        static Card DummyMonsterCard()
//        {
//            return new Card(
//                    System.Guid.NewGuid(),
//                    "Lazy Peon",
//                    "No work...",
//                    3,
//                    CardType.Monster,
//                    Element.Normal,
//                    Race.Orc,
//                    Rarity.Common);
//        }

//        static Card DummySpellCard()
//        {
//            return new Card(
//                    System.Guid.NewGuid(),
//                    "Flame Lance",
//                    "A fiery lance that not many mages are able to cast",
//                    5,
//                    CardType.Spell,
//                    Element.Fire,
//                    Race.None,
//                    Rarity.Rare);
//        }
//    }
//}
