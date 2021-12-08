namespace MTCG.BL.EndpointController
{
    public class TradeController : Controller
    {
        // Post => With trade body (Id, CardToTrade, CardInstanceId (not public), trade requirements (min damage, element, type, specific card))


        //// TODO - Transaction try catch
        //public bool Trade(Trade trade, TradeOffer offer)
        //{
        //    // Check if a user wants to trade with himself
        //    if (trade.UserOneID == trade.UserTwoID)
        //        return false;

        //    // Check if users have the cards
        //    var u1Stack = UserController.Instance.GetUserCardStack(trade.UserOneID);
        //    var u2Stack = UserController.Instance.GetUserCardStack(trade.UserTwoID);

        //    if (u1Stack.Find(ci => ci.ID == trade.CardOneID) == null ||
        //        u2Stack.Find(ci => ci.ID == trade.CardTwoID) == null)
        //        throw new System.Exception("One of the users of the current trade does not own the card to be traded!");

        //    // Check if the accepting card is the right card
        //    if (u2Stack.Find(c => c.ID == trade.CardTwoID && c.CardID == offer.WantedCardID) == null)
        //        throw new ArgumentException("The accepting party has selected the wrong card for this trade!");

        //    // Switch card 1
        //    NpgsqlCommand cmd = new("UPDATE user_cards SET card_instance_id=@card_instance_id WHERE user_id=@user_id;");
        //    cmd.Parameters.AddWithValue("card_instance_id", trade.CardOneID);
        //    cmd.Parameters.AddWithValue("user_id", trade.UserTwoID);
        //    Database.Instance.ExecuteNonQuery(cmd);

        //    // Switch card 2
        //    cmd.Parameters.Clear();
        //    cmd.Parameters.AddWithValue("card_instance_id", trade.CardTwoID);
        //    cmd.Parameters.AddWithValue("user_id", trade.UserOneID);
        //    Database.Instance.ExecuteNonQuery(cmd);

        //    // Insert trade
        //    cmd = new("INSERT INTO trades (id, card_one_id, user_one_id, card_two_id, user_two_id) VALUES (@id, @c1, @u1, @c2, @u2);");
        //    cmd.Parameters.AddWithValue("id", trade.ID);
        //    cmd.Parameters.AddWithValue("c1", trade.CardOneID);
        //    cmd.Parameters.AddWithValue("u1", trade.UserOneID);
        //    cmd.Parameters.AddWithValue("c2", trade.CardTwoID);
        //    cmd.Parameters.AddWithValue("u2", trade.UserTwoID);
        //    Database.Instance.ExecuteNonQuery(cmd);

        //    // Delete offer
        //    cmd = new("DELETE FROM offers WHERE id=@id;");
        //    cmd.Parameters.AddWithValue("id", offer.ID);
        //    Database.Instance.ExecuteNonQuery(cmd);

        //    return true;
        //}

        //public bool CreateOffer(TradeOffer offer)
        //{
        //    // Check if user has the card to offer
        //    var userStack = UserController.Instance.GetUserCardStack(offer.UserID);

        //    if (userStack.Find(c => c.ID == offer.OfferedCardID) == null)
        //        throw new System.Exception("The user that made the offer does not own this card!");

        //    // Find the card that is wanted (against invalid inputs)
        //    Card c = CardController.Instance.Select(offer.WantedCardID);

        //    if (c == null)
        //        throw new ArgumentException("The given wanted card id in the offer is not related to any card!");

        //    NpgsqlCommand cmd = new("INSERT INTO offers (id, user_id, offered_card_id, wanted_card_id) VALUES (@id, @user_id, @offered_card_id, @wanted_card_id);");
        //    cmd.Parameters.AddWithValue("id", offer.ID);
        //    cmd.Parameters.AddWithValue("user_id", offer.UserID);
        //    cmd.Parameters.AddWithValue("offered_card_id", offer.OfferedCardID);
        //    cmd.Parameters.AddWithValue("wanted_card_id", offer.WantedCardID);

        //    return Database.Instance.ExecuteNonQuery(cmd) == 1;
        //}
    }
}
