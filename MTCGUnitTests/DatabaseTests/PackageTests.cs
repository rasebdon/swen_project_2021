using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTCG.Controller;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCGUnitTests.DatabaseTests
{
    [TestClass]
    public class PackageTests
    {
        [TestMethod]
        public void CreatePackageTest()
        {
            // Create package
            Package package = PackageTests.CreateDummyPackage();

            try
            {
                // Insert cards
                CardController.Instance.InsertCards(package.Cards);
                // Insert package
                PackageController.Instance.AddPackage(package);

                var inserted = PackageController.Instance.GetPackage(package.ID);

                Assert.AreEqual(package.Cards.Count, inserted.Cards.Count);
                Assert.AreEqual(package.ID, inserted.ID);
            }
            finally
            {
                // Delete package
                PackageController.Instance.DeletePackage(package);
                // Delete cards
                CardController.Instance.DeleteCards(package.Cards);
            }
        }

        [TestMethod]
        public void BuyPackageTest()
        {
            // Create package
            Package package = PackageTests.CreateDummyPackage();
            // Insert cards
            CardController.Instance.InsertCards(package.Cards);
            // Insert package
            PackageController.Instance.AddPackage(package);
            // Create user
            User user = UserController.Instance.Register("dummy", "1234");

            try
            {
                // Acquire package
                bool success = UserController.Instance.BuyPackage(user, package.ID);

                Assert.IsTrue(success, "There was an error buying the package");
                Assert.AreEqual(user.Coins, UserController.Instance.GetUser("dummy").Coins, "Package cost were not subtracted correcty");
                Assert.AreEqual((int)Package.DrawnCardsAmount, UserController.Instance.GetUserCardStack(user.ID).Count, "Cards were not added to the user successfully");
            }
            finally
            {
                // Delete user
                UserController.Instance.DeleteUser(user);
                // Delete package
                PackageController.Instance.DeletePackage(package);
                // Delete cards
                CardController.Instance.DeleteCards(package.Cards);
            }
        }

        public static Package CreateDummyPackage()
        {
            List<Card> cards = new();
            // Add some cards
            cards.Add(
                new SpellCard(
                    "Flame Lance",
                    "A fiery lance that not many mages are able to cast",
                    5,
                    Element.Fire,
                    Rarity.Rare));
            cards.Add(
                new MonsterCard(
                    "Lazy Peon",
                    "No work...",
                    3,
                    Element.Normal,
                    Rarity.Common,
                    Race.Orc));
            cards.Add(
                new MonsterCard(
                    "Deathwing",
                    "All shall burn, beneath the shadow of my wings",
                    15,
                    Element.Fire,
                    Rarity.Legendary,
                    Race.Draconid));
            cards.Add(
                new MonsterCard(
                    "Elven Hunter",
                    "Is there something to hunt?",
                    4,
                    Element.Fire,
                    Rarity.Common,
                    Race.Elf));
            cards.Add(
                new SpellCard(
                    "Firestorm",
                    "Fire everything!",
                    10,
                    Element.Fire,
                    Rarity.Epic));

            return new Package("Dummy Package", "This is a dummy package", 5, cards);
        }
    }
}
