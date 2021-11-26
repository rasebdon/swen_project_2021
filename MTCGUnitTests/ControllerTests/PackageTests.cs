//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Collections.Generic;
//using System.Linq;

//namespace MTCGUnitTests.ControllerTests
//{
//    [TestClass]
//    public class PackageTests
//    {
//        [TestMethod]
//        public void CreatePackageTest()
//        {
//            // Create package
//            Package package = PackageTests.CreateDummyPackage();

//            try
//            {
//                // Insert cards
//                CardController.Instance.Insert(package.Cards);
//                // Insert package
//                PackageController.Instance.Insert(package);

//                var inserted = PackageController.Instance.Select(package.ID);

//                Assert.AreEqual(package.Cards.Count, inserted.Cards.Count);
//                Assert.AreEqual(package.ID, inserted.ID);
//            }
//            finally
//            {
//                // Delete package
//                PackageController.Instance.Delete(package);
//                // Delete cards
//                CardController.Instance.Delete(package.Cards);
//            }
//        }

//        [TestMethod]
//        public void BuyPackageTest()
//        {
//            // Create package
//            Package package = PackageTests.CreateDummyPackage();
//            // Insert cards
//            CardController.Instance.Insert(package.Cards);
//            // Insert package
//            PackageController.Instance.Insert(package);
//            // Create user
//            User user = UserController.Instance.Register("dummy", "1234");

//            try
//            {
//                // Acquire package
//                bool success = UserController.Instance.BuyPackage(user, package.ID).Count == 5;

//                Assert.IsTrue(success, "There was an error buying the package");
//                Assert.AreEqual(user.Coins, UserController.Instance.Select("dummy").Coins, "Package cost were not subtracted correcty");
//                Assert.AreEqual((int)Package.DrawnCardsAmount, UserController.Instance.GetUserCardStack(user.ID).Count, "Cards were not added to the user successfully");
//            }
//            finally
//            {
//                // Delete user
//                UserController.Instance.Delete(user);
//                // Delete package
//                PackageController.Instance.Delete(package);
//                // Delete cards
//                CardController.Instance.Delete(package.Cards);
//            }
//        }

//        public static Package CreateDummyPackage()
//        {
//            List<Card> cards = new();
//            // Add some cards
//            cards.Add(
//                new SpellCard(
//                    "Flame Lance",
//                    "A fiery lance that not many mages are able to cast",
//                    5,
//                    Element.Fire,
//                    Rarity.Rare));
//            cards.Add(
//                new MonsterCard(
//                    "Lazy Peon",
//                    "No work...",
//                    3,
//                    Element.Normal,
//                    Rarity.Common,
//                    Race.Orc));
//            cards.Add(
//                new MonsterCard(
//                    "Deathwing",
//                    "All shall burn, beneath the shadow of my wings",
//                    15,
//                    Element.Fire,
//                    Rarity.Legendary,
//                    Race.Draconid));
//            cards.Add(
//                new MonsterCard(
//                    "Elven Hunter",
//                    "Is there something to hunt?",
//                    4,
//                    Element.Fire,
//                    Rarity.Common,
//                    Race.Elf));
//            cards.Add(
//                new SpellCard(
//                    "Firestorm",
//                    "Fire everything!",
//                    10,
//                    Element.Fire,
//                    Rarity.Epic));

//            return new Package("Dummy Package", "This is a dummy package", 5, cards);
//        }
//    }
//}
