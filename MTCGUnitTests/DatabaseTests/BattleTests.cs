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
    public class BattleTests
    {
        [TestMethod]
        public void BattleTest()
        {
            User user1 = null;
            User user2 = null;

            try
            {
                // Create user
                user1 = UserController.Instance.Register("dummy1", "1234");
                user2 = UserController.Instance.Register("dummy2", "1234");

                MonsterCard strongCard = new(
                    "Strong Monster",
                    "This is a strong card",
                    200,
                    Element.Fire,
                    Rarity.Legendary,
                    Race.Draconid);
                SpellCard weakCard = new(
                    "Weak Spell",
                    "This is a weak card",
                    5,
                    Element.Water,
                    Rarity.Common);

                try
                {
                    // Insert cards
                    CardController.Instance.Insert(strongCard);
                    CardController.Instance.Insert(weakCard);

                    // Create instances
                    List<CardInstance> user1Cards = new()
                    {
                        new CardInstance(strongCard),
                        new CardInstance(strongCard),
                        new CardInstance(strongCard),
                        new CardInstance(strongCard)
                    };
                    List<CardInstance> user2Cards = new()
                    {
                        new CardInstance(weakCard),
                        new CardInstance(weakCard),
                        new CardInstance(weakCard),
                        new CardInstance(weakCard)
                    };
                    CardInstanceController.Instance.Insert(user1Cards);
                    CardInstanceController.Instance.Insert(user2Cards);

                    // Add cards to users
                    for (int i = 0; i < 4; i++)
                    {
                        UserController.Instance.AddCardToUser(user1, user1Cards[i]);
                        UserController.Instance.AddCardToUser(user2, user2Cards[i]);
                    }

                    // Create decks
                    Deck deckUser1 = new("Strong deck", user1.ID, user1Cards, true);
                    Deck deckUser2 = new("Weak deck", user2.ID, user2Cards, true);

                    try
                    {
                        // Insert decks
                        DeckController.Instance.Insert(deckUser1);
                        DeckController.Instance.Insert(deckUser2);

                        // Retrieve decks
                        deckUser1 = DeckController.Instance.Select(deckUser1.ID);
                        deckUser2 = DeckController.Instance.Select(deckUser2.ID);

                        // Battle
                        BattleResult result = BattleController.Instance.Battle(deckUser1, deckUser2);

                        Console.WriteLine(result.Log);

                        User winner = UserController.Instance.Select(result.Winner);
                        User loser = UserController.Instance.Select(result.Loser);

                        Assert.IsFalse(result.Tie, "Tie is not set correctly");
                        Assert.AreEqual((ushort)103, winner.ELO, "Elo of winner was not set correctly");
                        Assert.AreEqual((ushort)95, loser.ELO, "Elo of loser was not set correctly");
                        Assert.AreEqual(1, loser.PlayedGames, "Played games of winner was not set correctly");
                        Assert.AreEqual(1, winner.PlayedGames, "Played games of loser was not set correctly");
                        Assert.AreEqual(21, winner.Coins, "Coins of winner was not set correctly");
                    }
                    finally
                    {
                        DeckController.Instance.Delete(deckUser1);
                        DeckController.Instance.Delete(deckUser2);
                    }

                }
                finally
                {
                    CardController.Instance.Delete(strongCard);
                    CardController.Instance.Delete(weakCard);
                }
            }
            finally
            {
                UserController.Instance.Delete(user1);
                UserController.Instance.Delete(user2);
            }
        }
    }
}
