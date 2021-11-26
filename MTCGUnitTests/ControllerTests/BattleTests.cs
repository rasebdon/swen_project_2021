//using MTCG.Models;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace MTCGUnitTests.ControllerTests
//{
//    public class BattleTests
//    {
//        [Test]
//        public void BattleTest()
//        {
//            User user1 = null;
//            User user2 = null;

//            try
//            {
//                Create user
//                user1 = UserController.Instance.Register("dummy1", "1234");
//                user2 = UserController.Instance.Register("dummy2", "1234");

//                MonsterCard strongCard = new(
//                    "Strong Monster",
//                    "This is a strong card",
//                    200,
//                    Element.Fire,
//                    Rarity.Legendary,
//                    Race.Draconid);
//                SpellCard weakCard = new(
//                    "Weak Spell",
//                    "This is a weak card",
//                    5,
//                    Element.Water,
//                    Rarity.Common);

//                try
//                {
//                    Insert cards
//                    CardController.Instance.Insert(strongCard);
//                    CardController.Instance.Insert(weakCard);

//                    Create instances
//                    List<CardInstance> user1Cards = new()
//                    {
//                        new CardInstance(strongCard),
//                        new CardInstance(strongCard),
//                        new CardInstance(strongCard),
//                        new CardInstance(strongCard)
//                    };
//                    List<CardInstance> user2Cards = new()
//                    {
//                        new CardInstance(weakCard),
//                        new CardInstance(weakCard),
//                        new CardInstance(weakCard),
//                        new CardInstance(weakCard)
//                    };
//                    CardInstanceController.Instance.Insert(user1Cards);
//                    CardInstanceController.Instance.Insert(user2Cards);

//                    Add cards to users
//                    for (int i = 0; i < 4; i++)
//                    {
//                        UserController.Instance.AddCardToUser(user1, user1Cards[i]);
//                        UserController.Instance.AddCardToUser(user2, user2Cards[i]);
//                    }

//                    Create decks
//                    Deck deckUser1 = new("Strong deck", user1.ID, user1Cards, true);
//                    Deck deckUser2 = new("Weak deck", user2.ID, user2Cards, true);

//                    try
//                    {
//                        Insert decks
//                        DeckController.Instance.Insert(deckUser1);
//                        DeckController.Instance.Insert(deckUser2);

//                        Retrieve decks(Matchmaking)
//                        deckUser1 = DeckController.Instance.Select(deckUser1.ID);
//                        deckUser2 = BattleController.Instance.FindMatch(deckUser1);

//                        Battle
//                       BattleResult result = BattleController.Instance.Battle(deckUser1, deckUser2);

//                        Console.WriteLine(result.Log);

//                        User winner = UserController.Instance.Select(result.Winner);
//                        User loser = UserController.Instance.Select(result.Loser);

//                        Assert.IsFalse(result.Tie, "Tie is not set correctly");
//                        Assert.AreEqual((ushort)103, winner.ELO, "Elo of winner was not set correctly");
//                        Assert.AreEqual((ushort)95, loser.ELO, "Elo of loser was not set correctly");
//                        Assert.AreEqual(1, loser.PlayedGames, "Played games of winner was not set correctly");
//                        Assert.AreEqual(1, winner.PlayedGames, "Played games of loser was not set correctly");
//                        Assert.AreEqual(21, winner.Coins, "Coins of winner was not set correctly");
//                    }
//                    finally
//                    {
//                        DeckController.Instance.Delete(deckUser1);
//                        DeckController.Instance.Delete(deckUser2);
//                    }

//                }
//                finally
//                {
//                    CardController.Instance.Delete(strongCard);
//                    CardController.Instance.Delete(weakCard);
//                }
//            }
//            finally
//            {
//                UserController.Instance.Delete(user1);
//                UserController.Instance.Delete(user2);
//            }
//        }

//        [TestMethod]
//        public void MonsterBattleModifierTest()
//        {
//            CharStream log = new();

//            Dragon should lose
//            bool dragonLose = BattleController.Instance.LoseDueToSpecialty(Race.Draconid, Race.FireElf, log);
//            bool fireElfLose = BattleController.Instance.LoseDueToSpecialty(Race.FireElf, Race.Draconid, log);

//            Orc should lose
//            bool orcLose = BattleController.Instance.LoseDueToSpecialty(Race.Orc, Race.Mage, log);
//            bool mageLose = BattleController.Instance.LoseDueToSpecialty(Race.Mage, Race.Orc, log);

//            Goblin should lose
//            bool goblinLose = BattleController.Instance.LoseDueToSpecialty(Race.Goblin, Race.Draconid, log);
//            bool dragonGoblinLose = BattleController.Instance.LoseDueToSpecialty(Race.Draconid, Race.Goblin, log);

//            Verify results
//            Assert.IsTrue(dragonLose);
//            Assert.IsFalse(fireElfLose);
//            Assert.IsTrue(orcLose);
//            Assert.IsFalse(mageLose);
//            Assert.IsTrue(goblinLose);
//            Assert.IsFalse(dragonGoblinLose);
//        }

//        [TestMethod]
//        public void SpellElementModifierTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod]
//        public void MixedFightModifierTest()
//        {
//            Assert.Fail();
//        }
//    }
//}
