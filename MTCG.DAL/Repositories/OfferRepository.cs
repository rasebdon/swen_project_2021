using MTCG.Models;
using Npgsql;
using System.Collections.Specialized;

namespace MTCG.DAL.Repositories
{
    public class OfferRepository : IRepository<TradeOffer>
    {
        private readonly IDatabase _db;
        private readonly ILog _log;

        public OfferRepository(IDatabase db, ILog log)
        {
            _db = db;
            _log = log;
        }

        public bool Delete(Guid id)
        {
            try
            {
                NpgsqlCommand cmd = new("DELETE FROM offers WHERE id=@id;");
                cmd.Parameters.AddWithValue("id", id);
                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public IEnumerable<TradeOffer> GetAll()
        {
            NpgsqlCommand cmd = new("SELECT * FROM offers;");

            OrderedDictionary[] offersRows = _db.Select(cmd);
            List<TradeOffer> offers = new();

            foreach (var row in offersRows)
            {
                TradeOffer? trade = ParseFromRow(row, _log);
                
                if (trade != null)
                    offers.Add(trade);
            }

            return offers;
        }

        public TradeOffer? GetById(Guid id)
        {
            try
            {
                NpgsqlCommand cmd = new("SELECT * FROM offers WHERE id=@id;");
                cmd.Parameters.AddWithValue("id", id);

                OrderedDictionary row = _db.SelectSingle(cmd);

                return ParseFromRow(row, _log);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return null;
            }
        }

        public bool Insert(TradeOffer offer)
        {
            try
            {
                NpgsqlCommand cmd = new(
                    @"INSERT INTO offers (id, user_id, offered_card_id, wanted_card_id)
                    VALUES (@id, @user_id, @offered_id, @wanted_id);");

                cmd.Parameters.AddWithValue("id", offer.ID);
                cmd.Parameters.AddWithValue("user_id", offer.UserID);
                cmd.Parameters.AddWithValue("offered_id", offer.OfferedCardInstanceId);
                cmd.Parameters.AddWithValue("wanted_id", offer.WantedCardID);

                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool Update(TradeOffer offer)
        {
            try
            {
                NpgsqlCommand cmd = new(
                    @"UPDATE trades
                    SET user_id=@user_id, offered_card_id=@offered_id, wanted_card_id=@wanted_id
                    WHERE id=@id;");

                cmd.Parameters.AddWithValue("id", offer.ID);
                cmd.Parameters.AddWithValue("user_id", offer.UserID);
                cmd.Parameters.AddWithValue("offered_id", offer.OfferedCardInstanceId);
                cmd.Parameters.AddWithValue("wanted_id", offer.WantedCardID);

                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public static TradeOffer? ParseFromRow(
            OrderedDictionary row,
            ILog log)
        {
            try
            {
                return new TradeOffer(
                    Guid.Parse(row["id"]?.ToString() ?? ""),
                    Guid.Parse(row["user_id"]?.ToString() ?? ""),
                    Guid.Parse(row["offered_card_id"]?.ToString() ?? ""),
                    Guid.Parse(row["wanted_card_id"]?.ToString() ?? ""));
            }
            catch (Exception ex)
            {
                log.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
