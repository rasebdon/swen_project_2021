//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Newtonsoft.Json;
//using System.Linq;

//namespace MTCGUnitTests.SerializationTests
//{
//    [TestClass]
//    public class PackageSerialization
//    {
//        [TestMethod]
//        public void SerializePackageWithoutID()
//        {
//            string json = "{\r\n    \"Name\": \"Dummy Package\",\r\n    \"Description\": \"This is a dummy package\",\r\n    \"Cost\": 5,\r\n    \"Cards\": [\r\n        {\r\n            \"Name\": \"Flame Lance\",\r\n            \"Damage\": 5,\r\n            \"CardType\": 1,\r\n            \"Element\": 1,\r\n            \"Rarity\": 1,\r\n            \"Description\": \"A fiery lance that not many mages are able to cast\"\r\n        },\r\n        {\r\n            \"Name\": \"Lazy Peon\",\r\n            \"Damage\": 3,\r\n            \"CardType\": 0,\r\n            \"Element\": 0,\r\n            \"Rarity\": 0,\r\n            \"Description\": \"No work...\"\r\n        },\r\n        {\r\n            \"Name\": \"Deathwing\",\r\n            \"Damage\": 15,\r\n            \"CardType\": 0,\r\n            \"Element\": 1,\r\n            \"Rarity\": 3,\r\n            \"Description\": \"All shall burn, beneath the shadow of my wings\"\r\n        },\r\n        {\r\n            \"Name\": \"Elven Hunter\",\r\n            \"Damage\": 4,\r\n            \"CardType\": 0,\r\n            \"Element\": 1,\r\n            \"Rarity\": 0,\r\n            \"Description\": \"Is there something to hunt?\"\r\n        },\r\n        {\r\n            \"Name\": \"Firestorm\",\r\n            \"Damage\": 10,\r\n            \"CardType\": 1,\r\n            \"Element\": 1,\r\n            \"Rarity\": 2,\r\n            \"Description\": \"Fire everything!\"\r\n        }\r\n    ]\r\n}";
//            var cardConverter = new CardConverter();

//            Package serialized = JsonConvert.DeserializeObject<Package>(json, cardConverter);

//            Assert.AreEqual(typeof(Package), serialized.GetType());
//            Assert.AreEqual(5, serialized.Cards.Count);
//        }

//        [TestMethod]
//        public void SerializePackageWithID()
//        {
//            string json = "{\r\n    \"Name\": \"Dummy Package\",\r\n    \"Description\": \"This is a dummy package\",\r\n    \"Cost\": 5,\r\n    \"Cards\": [\r\n        {\r\n            \"Name\": \"Flame Lance\",\r\n            \"Damage\": 5,\r\n            \"CardType\": 1,\r\n            \"Element\": 1,\r\n            \"Rarity\": 1,\r\n            \"Description\": \"A fiery lance that not many mages are able to cast\",\r\n            \"ID\": \"559ecb4b-f262-49c1-b79e-bae8e3d955e1\"\r\n        },\r\n        {\r\n            \"Name\": \"Lazy Peon\",\r\n            \"Damage\": 3,\r\n            \"CardType\": 0,\r\n            \"Element\": 0,\r\n            \"Rarity\": 0,\r\n            \"Description\": \"No work...\",\r\n            \"ID\": \"36f26e4f-0ed7-4632-b06f-032128439ced\"\r\n        },\r\n        {\r\n            \"Name\": \"Deathwing\",\r\n            \"Damage\": 15,\r\n            \"CardType\": 0,\r\n            \"Element\": 1,\r\n            \"Rarity\": 3,\r\n            \"Description\": \"All shall burn, beneath the shadow of my wings\",\r\n            \"ID\": \"aeea6393-a4c6-48e0-9700-23dff952aea5\"\r\n        },\r\n        {\r\n            \"Name\": \"Elven Hunter\",\r\n            \"Damage\": 4,\r\n            \"CardType\": 0,\r\n            \"Element\": 1,\r\n            \"Rarity\": 0,\r\n            \"Description\": \"Is there something to hunt?\",\r\n            \"ID\": \"81959401-0651-4b73-aa9b-5b94478c26db\"\r\n        },\r\n        {\r\n            \"Name\": \"Firestorm\",\r\n            \"Damage\": 10,\r\n            \"CardType\": 1,\r\n            \"Element\": 1,\r\n            \"Rarity\": 2,\r\n            \"Description\": \"Fire everything!\",\r\n            \"ID\": \"2de6d042-8fcc-400c-ba13-8d38669aebbe\"\r\n        }\r\n    ],\r\n    \"ID\": \"a441338c-4ef4-4139-84f5-6964095ac467\"\r\n}";

//            Package serialized = PackageConverter.Parse(json);

//            Assert.AreEqual(typeof(Package), serialized.GetType());
//            Assert.AreEqual(5, serialized.Cards.Count);
//            Assert.AreEqual("559ecb4b-f262-49c1-b79e-bae8e3d955e1", serialized.Cards[0].ID.ToString(), "Card ids are not identical!");
//            Assert.AreEqual("a441338c-4ef4-4139-84f5-6964095ac467", serialized.ID.ToString(), "Package ids are not identical!");
//        }

//    }
//}
