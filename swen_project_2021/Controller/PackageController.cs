using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
using MTCG.Http;
using Npgsql;

namespace MTCG.Controller
{
    /// <summary>
    /// This controller manages the package related functions
    /// </summary>
    class PackageController : Singleton<PackageController>
    {

        public bool AddPackage(Package package, HttpAuthorization auth)
        {
            // Check if auth is related to an admin
            User user = UserController.Instance.Authenticate(auth);

            if (!user.IsAdmin)
                return false;

            // Insert package data
            string sql = "INSERT INTO packages (ID, name, description, cost) VALUES (@ID, @name, @description, @cost);";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("ID", package.ID);
            cmd.Parameters.AddWithValue("name", package.Name);
            cmd.Parameters.AddWithValue("description", package.Description);
            cmd.Parameters.AddWithValue("cost", package.Cost);

            bool error = Server.Instance.Database.ExecuteNonQuery(cmd) != 1;
            if (error)
                throw new Exception("Fatal error inserting package!");

            int errn = 0;
            // Insert card links
            for (int i = 0; i < package.Cards.Count; i++)
            {
                errn += LinkPackageCard(package.ID, package.Cards[i].ID) ? 0 : 1;
            }

            if(errn != 0)
                throw new Exception("Fatal error linking package with cards!");

            return true;
        }

        public bool LinkPackageCard(Guid packageID, Guid cardID)
        {
            string sql = "INSERT INTO package_cards (packageID, cardID) VALUES (@packageID, @cardID);";
            NpgsqlCommand cmd = new(sql);

            cmd.Parameters.AddWithValue("packageID", packageID.ToString());
            cmd.Parameters.AddWithValue("cardID", cardID.ToString());

            return Server.Instance.Database.ExecuteNonQuery(cmd) == 1;
        }

        public Package GetPackage(uint packageId)
        {
            // Get the package from the table
            string sql = "SELECT * FROM packages WHERE ID=@ID";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("ID", packageId);
            var packageRow = Server.Instance.Database.SelectSingle(cmd);

            // Get the package cards the table
            sql = "SELECT * FROM package_cards WHERE packageID=@ID";
            cmd = new(sql);
            cmd.Parameters.AddWithValue("ID", packageId);
            var packageCardsRows = Server.Instance.Database.Select(cmd);

            return new Package(packageRow, packageCardsRows);
        }

        public List<Card> GetCardsWithRarity(List<Card> _cards, Rarity rarity)
        {
            // Get the cards with the correct rarity
            List<Card> correctRarity = _cards.FindAll(c => c.Rarity == rarity);
            if (correctRarity.Count == 0)
                return null;
            return correctRarity;
        }

        public Card GetRandomCardWithRarity(List<Card> cards, Rarity rarity)
        {
            var c = GetCardsWithRarity(cards, rarity);
            if (c == null)
                return null;
            return c[new Random().Next(0, c.Count)];
        }
        public CardInstance GetRandomCardInstanceWithRarity(List<Card> cards, Rarity rarity)
        {
            var c = GetRandomCardWithRarity(cards, rarity);
            if (c == null)
                return null;
            return new CardInstance(c);
        }

        public List<CardInstance> OpenPackage(Package package)
        {
            List<CardInstance> drawnCards = new();

            // Draw cards (which card will be drawn depends on its rarity)
            for (int i = 0; i < Package.DrawnCardsAmount; i++)
            {
                CardInstance card = null;
                // Check if a card was drawn, if not, repeat
                while (card == null)
                {
                    // Roll
                    int roll = new Random().Next(0, 100);
                    if (roll < 50)
                    {
                        card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Common);
                    }
                    else if (roll < 75)
                    {
                        card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Legendary);
                    }
                    else if (roll < 90)
                    {
                        card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Epic);
                    }
                    else
                    {
                        card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Legendary);
                    }
                }
                drawnCards.Add(card);
            }

            // Add drawn cards to database
            if(!CardController.Instance.InsertCardInstances(drawnCards))
            {
                ServerLog.Print("An error occured while drawing cards! Redrawing...", ServerLog.OutputFormat.Error);

                // Error occured, delete drawn cards
                CardController.Instance.DeleteCardInstances(drawnCards);

                // Redraw cards
                return OpenPackage(package);
            }

            return drawnCards;
        }
    }
}
