using MTCG.Controller.Exceptions;
using MTCG.Interfaces;
using MTCG.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MTCG.Controller
{
    /// <summary>
    /// This controller manages the package related functions
    /// </summary>
    public class PackageController : Singleton<PackageController>, ISelectable<Package>, IInsertable<Package>
    {
        public PackageController() { }
        
        // Select
        public Package Select(Guid packageId)
        {
            // Get the package from the table
            string sql = "SELECT * FROM packages WHERE id=@id";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("id", packageId);
            var packageRow = Database.Instance.SelectSingle(cmd);

            // Get the package cards the table
            sql = "SELECT * FROM package_cards, cards WHERE package_id=@id AND cards.id=package_cards.card_id";
            cmd = new(sql);
            cmd.Parameters.AddWithValue("id", packageId);
            var packageCardsRows = Database.Instance.SelectAsync(cmd);

            if (packageCardsRows == null)
                throw new NullReferenceException($"There are no cards in the package {packageId}!");

            return new Package(packageRow, packageCardsRows.Result);
        }
        public Package Select(string packageName)
        {
            // Get the package from the table
            string sql = "SELECT * FROM packages WHERE name=@packageName";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("packageName", packageName);
            var packageRow = Database.Instance.SelectSingle(cmd);

            if (packageRow == null || packageRow["id"] == null)
                throw new NoEntryFoundException(sql);

            // Get the package cards the table
            cmd = new("SELECT * FROM package_cards, cards WHERE package_id=@id AND cards.id=package_cards.card_id");
            cmd.Parameters.AddWithValue("id", packageRow["id"]);
            var packageCardsRows = Database.Instance.SelectAsync(cmd);

            if (packageCardsRows == null)
                throw new NullReferenceException($"There are no cards in the package {packageRow["id"]}!");

            return new Package(packageRow, packageCardsRows.Result);
        }

        // Insert
        public bool Insert(Package package)
        {
            // Insert package data
            string sql = "INSERT INTO packages (id, name, description, cost) VALUES (@id, @name, @description, @cost);";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("id", package.ID);
            cmd.Parameters.AddWithValue("name", package.Name);
            cmd.Parameters.AddWithValue("description", package.Description);
            cmd.Parameters.AddWithValue("cost", (int)package.Cost);

            bool error = Database.Instance.ExecuteNonQuery(cmd) != 1;
            if (error)
                throw new Exception("Fatal error inserting package!");

            int errn = 0;
            // Insert card links
            for (int i = 0; i < package.Cards.Count; i++)
            {
                errn += LinkPackageCard(package.ID, package.Cards[i].ID) ? 0 : 1;
            }

            if (errn != 0)
                throw new Exception("Fatal error linking package with cards!");

            return true;
        }
        public bool LinkPackageCard(Guid packageID, Guid cardID)
        {
            string sql = "INSERT INTO package_cards (package_id, card_id) VALUES (@packageID, @cardID);";
            NpgsqlCommand cmd = new(sql);

            cmd.Parameters.AddWithValue("packageID", packageID);
            cmd.Parameters.AddWithValue("cardID", cardID);

            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }

        // Delete
        public bool Delete(Package package)
        {
            NpgsqlCommand cmd = new("DELETE FROM packages WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", package.ID);
            return Database.Instance.ExecuteNonQuery(cmd) == 1;
        }

        // Helper Methods
        private List<Card> GetCardsWithRarity(List<Card> _cards, Rarity rarity)
        {
            // Get the cards with the correct rarity
            List<Card> correctRarity = _cards.FindAll(c => c.Rarity == rarity);
            if (correctRarity.Count == 0)
                return null;
            return correctRarity;
        }
        private Card GetRandomCardWithRarity(List<Card> cards, Rarity rarity)
        {
            var c = GetCardsWithRarity(cards, rarity);
            if (c == null)
                return null;
            return c[new Random().Next(0, c.Count)];
        }
        private CardInstance GetRandomCardInstanceWithRarity(List<Card> cards, Rarity rarity)
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
                    if (roll > 99)
                    {
                        card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Legendary);
                    }
                    else if (roll > 90)
                    {
                        card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Epic);
                    }
                    else if (roll > 75)
                    {
                        card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Rare);
                    }
                    else
                    {
                        card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Common);
                    }
                }
                drawnCards.Add(card);
            }

            // Add drawn cards to database
            if (!CardInstanceController.Instance.Insert(drawnCards))
            {
                ServerLog.WriteLine("An error occured while drawing cards! Redrawing...", ServerLog.OutputFormat.Error);

                // Error occured, delete drawn cards
                CardInstanceController.Instance.Delete(drawnCards);

                // Redraw cards
                return OpenPackage(package);
            }

            return drawnCards;
        }
    }
}
