using MTCG.Models;
using Npgsql;
using System.Collections.Specialized;

namespace MTCG.DAL.Repositories
{
    public class PackageRepository : IRepository<Package>
    {
        private readonly IDatabase _db;
        private readonly ILog _log;

        public PackageRepository(IDatabase db, ILog log)
        {
            _db = db;
            _log = log;
        }

        public bool Delete(Guid id)
        {
            NpgsqlCommand cmd = new("DELETE FROM packages WHERE id=@id;");
            cmd.Parameters.AddWithValue("id", id);
            return _db.ExecuteNonQuery(cmd) == 1;
        }

        public Package? GetById(Guid id)
        {
            // Get the package from the table
            string sql = "SELECT * FROM packages WHERE id=@id;";
            NpgsqlCommand cmd = new(sql);
            cmd.Parameters.AddWithValue("id", id);
            OrderedDictionary packageRow = _db.SelectSingle(cmd);

            // Get the package cards the table
            sql = "SELECT * FROM package_cards, cards WHERE package_id=@id AND cards.id=package_cards.card_id;";
            cmd = new(sql);
            cmd.Parameters.AddWithValue("id", id);
            OrderedDictionary[] packageCardRows = _db.Select(cmd);

            return ParseFromRow(packageRow, packageCardRows, _log);
        }

        public static Package? ParseFromRow(OrderedDictionary packageRow, OrderedDictionary[] cardRows, ILog _log)
        {
            try
            {
                List<Card> cards = new();
                // Parse list
                foreach (OrderedDictionary row in cardRows)
                {
                    Card? card;
                    if ((card = CardRepository.ParseFromRow(row, _log)) != null)
                        cards.Add(card);
                }

                return new Package(
                    Guid.Parse(packageRow?["id"]?.ToString() ?? ""),
                    packageRow?["name"]?.ToString() ?? "",
                    packageRow?["description"]?.ToString() ?? "",
                    ushort.Parse(packageRow?["cost"]?.ToString() ?? ""),
                    cards);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.Message);
                return null;
            }
        }

        public bool Insert(Package package)
        {
            try
            {
                // Insert package data
                NpgsqlCommand cmd = new(
                    @"INSERT INTO packages (id, name, description, cost) 
                    VALUES (@id, @name, @description, @cost);");
                cmd.Parameters.AddWithValue("id", package.ID);
                cmd.Parameters.AddWithValue("name", package.Name);
                cmd.Parameters.AddWithValue("description", package.Description);
                cmd.Parameters.AddWithValue("cost", (int)package.Cost);

                if (_db.ExecuteNonQuery(cmd) != 1)
                    throw new Exception("Fatal error inserting package!");

                // Insert card link
                cmd = new("INSERT INTO package_cards (package_id, card_id) VALUES (@package_id, @card_id);");
                cmd.Parameters.Add(new NpgsqlParameter("package_id", NpgsqlTypes.NpgsqlDbType.Uuid));
                cmd.Parameters.Add(new NpgsqlParameter("card_id", NpgsqlTypes.NpgsqlDbType.Uuid));

                for (int i = 0; i < package.Cards.Count; i++)
                {
                    cmd.Parameters["package_id"].Value = package.ID;
                    cmd.Parameters["card_id"].Value = package.Cards[i].ID;

                    if (_db.ExecuteNonQuery(cmd) != 1)
                    {
                        throw new Exception("Fatal error linking package with cards!");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                // Revert changes
                Delete(package.ID);
                return false;
            }
        }

        public bool Update(Package entityOld, Package entityNew)
        {
            throw new NotImplementedException();
        }
    }
}
