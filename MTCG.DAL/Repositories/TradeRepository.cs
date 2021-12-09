using MTCG.Models;
using Npgsql;
using System.Collections.Specialized;

namespace MTCG.DAL.Repositories
{
    public class TradeRepository : IRepository<Trade>
    {
        private readonly IDatabase _db;
        private readonly ILog _log;

        public TradeRepository(IDatabase db, ILog log)
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

        /// <summary>
        /// Also switches the cards owners!
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        public bool Insert(Trade trade)
        {
            try
            {
                List<TransactionObject> transaction = new();

                // Insert trade
                NpgsqlCommand cmd1 = new(
                    @"INSERT INTO trades (id, card_one_id, user_one_id, card_two_id, user_two_id)
                    VALUES (@id, @card_one_id, @user_one_id, @card_two_id, @user_two_id);");
                cmd1.Parameters.AddWithValue("id", trade.ID);
                cmd1.Parameters.AddWithValue("card_one_id", trade.OfferedCardInstanceId);
                cmd1.Parameters.AddWithValue("user_one_id", trade.OfferUserId);
                cmd1.Parameters.AddWithValue("card_two_id", trade.WantedCardInstanceId);
                cmd1.Parameters.AddWithValue("user_two_id", trade.AcceptUserId);
                transaction.Add(new(cmd1, 1));

                // Delete cards from stacks
                NpgsqlCommand cmd2 = new(
                    @"DELETE FROM user_cards 
                    WHERE (card_instance_id=@offered_card_id AND user_id=@offer_user_id)
                    OR (card_instance_id=@wanted_card_id AND user_id=@accept_user_id);");
                cmd2.Parameters.AddWithValue("offered_card_id", trade.OfferedCardInstanceId);
                cmd2.Parameters.AddWithValue("offer_user_id", trade.OfferUserId);
                cmd2.Parameters.AddWithValue("wanted_card_id", trade.WantedCardInstanceId);
                cmd2.Parameters.AddWithValue("accept_user_id", trade.AcceptUserId);
                transaction.Add(new(cmd2, 2));

                // Second insert wanted card into offered card user stack
                NpgsqlCommand cmd3 = new(
                    @"INSERT INTO user_cards (user_id, card_instance_id) VALUES (@offer_user_id, @wanted_card_id);
                      INSERT INTO user_cards (user_id, card_instance_id) VALUES (@accept_user_id, @offered_card_id);");
                cmd3.Parameters.AddWithValue("wanted_card_id", trade.WantedCardInstanceId);
                cmd3.Parameters.AddWithValue("offer_user_id", trade.OfferUserId);
                cmd3.Parameters.AddWithValue("accept_user_id", trade.AcceptUserId);
                cmd3.Parameters.AddWithValue("offered_card_id", trade.OfferedCardInstanceId);
                transaction.Add(new(cmd3, 2));

                return _db.ExecuteNonQueryTransaction(transaction);
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
                cmd.Parameters.AddWithValue("c1", trade.OfferedCardInstanceId);
                cmd.Parameters.AddWithValue("u1", trade.OfferUserId);
                cmd.Parameters.AddWithValue("c2", trade.WantedCardInstanceId);
                cmd.Parameters.AddWithValue("u2", trade.AcceptUserId);

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
