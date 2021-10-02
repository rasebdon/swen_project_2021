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
    public class TradeTests
    {
        [TestMethod]
        public void TradeTest()
        {
            // Create users
            User u1 = UserController.Instance.Register("dummy1", "1234");
            User u2 = UserController.Instance.Register("dummy2", "1234");

            try
            {
                // Create cards
                Card c1 = new SpellCard(
                        "Flame Lance",
                        "A fiery lance that not many mages are able to cast",
                        5,
                        Element.Fire,
                        Rarity.Rare);
                Card c2 = new MonsterCard(
                        "Lazy Peon",
                        "No work...",
                        3,
                        Element.Normal,
                        Rarity.Common,
                        Race.Orc);
                CardController.Instance.Insert(c1);
                CardController.Instance.Insert(c2);

                try
                {
                    // Add cards
                    var u1c1instance = new CardInstance(c1);
                    var u2c2instance = new CardInstance(c2);
                    CardInstanceController.Instance.Insert(u1c1instance);
                    CardInstanceController.Instance.Insert(u2c2instance);

                    try
                    {
                        // Link cards to user
                        UserController.Instance.AddCardToUser(u1, u1c1instance);
                        UserController.Instance.AddCardToUser(u2, u2c2instance);

                        // Create trade offer
                        TradeOffer offer = new(u1, u1c1instance, c2); 
                        TradeController.Instance.CreateOffer(offer);

                        // Accept trade offer
                        Trade trade = new(u1, u1c1instance, u2, u2c2instance);
                        bool success = TradeController.Instance.Trade(trade, offer);

                        Assert.IsTrue(success);

                        var newStackU1 = UserController.Instance.GetUserCardStack(u1.ID);
                        var newStackU2 = UserController.Instance.GetUserCardStack(u2.ID);

                        Assert.IsNotNull(newStackU1.Find(c => c.ID == u2c2instance.ID));
                        Assert.IsNotNull(newStackU2.Find(c => c.ID == u1c1instance.ID));
                    }
                    finally
                    {
                        CardInstanceController.Instance.Delete(u1c1instance);
                        CardInstanceController.Instance.Delete(u2c2instance);
                    }
                }
                finally
                {
                    CardController.Instance.Delete(c1);
                    CardController.Instance.Delete(c2);
                }
            }
            finally
            {
                UserController.Instance.Delete(u1.ID);
                UserController.Instance.Delete(u2.ID);
            }
        }
    }
}
