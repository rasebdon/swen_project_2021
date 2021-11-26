namespace MTCG.Models
{
    public class TradeOffer : DataObject
    {
        public Guid UserID { get; }
        public Guid OfferedCardID { get; }
        public Guid WantedCardID { get; }

        public TradeOffer(Guid id, User user, CardInstance offeredCard, Card wantedCard) : base(id)
        {
            UserID = user.ID;
            OfferedCardID = offeredCard.ID;
            WantedCardID = wantedCard.ID;
        }
    }
}
