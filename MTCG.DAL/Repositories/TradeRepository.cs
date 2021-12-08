using MTCG.Models;
using Npgsql;
using System.Collections.Specialized;

namespace MTCG.DAL.Repositories
{
    public class TradedRepository : IRepository<Trade>
    {
        private readonly IDatabase _db;
        private readonly ILog _log;

        public TradedRepository(IDatabase db, ILog log)
        {
            _db = db;
            _log = log;
        }

        public bool Delete(Guid id)
        {
            try
            {
                NpgsqlCommand cmd = new("DELETE FROM trades WHERE id=@id;");
                cmd.Parameters.AddWithValue("id", id);
                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public IEnumerable<Trade> GetAll()
        {
            NpgsqlCommand cmd = new("SELECT * FROM trades;");

            OrderedDictionary[] tradeRows = _db.Select(cmd);
            List<Trade> trades = new();

            foreach (var row in tradeRows)
            {
                cmd = new("SELECT * FROM users WHERE Id=@id;");
                cmd.Parameters.Add("id", NpgsqlTypes.NpgsqlDbType.Uuid);

                Trade? trade = ParseFromRow(row, _log);
                
                if (trade != null)
                    trades.Add(trade);
            }

            return trades;
        }

        public Trade? GetById(Guid id)
        {
            try
            {
                NpgsqlCommand cmd = new("SELECT * FROM trades WHERE id=@id;");
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

        public bool Insert(Trade trade)
        {
            try
            {
                NpgsqlCommand cmd = new(
                    @"INSERT INTO trades (id, card_one_id, user_one_id, card_two_id, user_two_id)
                    VALUES (@id, @c1, @u1, @c2, @u2);");

                cmd.Parameters.AddWithValue("id", trade.ID);
                cmd.Parameters.AddWithValue("c1", trade.CardOneID);
                cmd.Parameters.AddWithValue("u1", trade.UserOneID);
                cmd.Parameters.AddWithValue("c2", trade.CardTwoID);
                cmd.Parameters.AddWithValue("u2", trade.UserTwoID);

                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool Update(Trade trade)
        {
            try
            {
                NpgsqlCommand cmd = new(
                    @"UPDATE trades SET card_one_id=@c1, card_two_id=@c2, user_one_id=@u1, user_two_id=@u2
                    WHERE id=@id;");

                cmd.Parameters.AddWithValue("id", trade.ID);
                cmd.Parameters.AddWithValue("c1", trade.CardOneID);
                cmd.Parameters.AddWithValue("u1", trade.UserOneID);
                cmd.Parameters.AddWithValue("c2", trade.CardTwoID);
                cmd.Parameters.AddWithValue("u2", trade.UserTwoID);

                return _db.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

        public static Trade? ParseFromRow(
            OrderedDictionary row,
            ILog log)
        {
            try
            {
                return new Trade(
                    Guid.Parse(row["id"]?.ToString() ?? ""),
                    Guid.Parse(row["user_one_id"]?.ToString() ?? ""),
                    Guid.Parse(row["card_one_id"]?.ToString() ?? ""),
                    Guid.Parse(row["user_two_id"]?.ToString() ?? ""),
                    Guid.Parse(row["card_two_id"]?.ToString() ?? ""));
            }
            catch (Exception ex)
            {
                log.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
