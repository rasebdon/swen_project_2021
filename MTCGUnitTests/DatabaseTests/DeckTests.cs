﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTCG.Controller;
using MTCG.Models;

namespace MTCGUnitTests.DatabaseTests
{
    [TestClass]
    public class DeckTests
    {
        [TestMethod]
        public void AddDeck()
        {
            // Create users
            User user = UserController.Instance.Register("dummy1", "1234");

            try
            {
                Package package = PackageTests.CreateDummyPackage();

                try
                {
                    // Add and buy Package
                    CardController.Instance.Insert(package.Cards);
                    PackageController.Instance.Insert(package);
                    UserController.Instance.BuyPackage(user.ID, package.ID);

                    // Create deck
                    List<CardInstance> stack = UserController.Instance.GetUserCardStack(user.ID);
                    Deck deck = new(
                        "Dummy Deck",
                        user.ID,
                        new List<CardInstance>()
                        {
                                stack[0],
                                stack[1],
                                stack[2],
                                stack[3]
                        });

                    try
                    {
                        bool success = DeckController.Instance.Insert(deck);

                        Assert.IsTrue(success);

                        // Retrieve deck
                        Deck inserted = DeckController.Instance.Select(deck.ID);

                        // Get user decks
                        List<Deck> decks = UserController.Instance.GetUserDecks(user);

                        Assert.AreEqual(1, decks.Count);
                        Assert.AreEqual(deck.ID, inserted.ID);
                        Assert.IsNotNull(decks.Find(d => d.ID == inserted.ID));
                    }
                    finally
                    {
                        DeckController.Instance.Delete(deck);
                    }
                }
                finally
                {
                    PackageController.Instance.Delete(package);
                    CardController.Instance.Delete(package.Cards);
                }
            }
            finally
            {
                UserController.Instance.Delete(user.ID);
            }
        }
    }
}
