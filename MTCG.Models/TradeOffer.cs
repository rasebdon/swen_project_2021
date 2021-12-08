namespace MTCG.Models
{
    public class TradeOffer : DataObject
    {
        public Guid UserID { get; }
        public Guid OfferedCardID { get; }
        public Guid WantedCardID { get; }

        public TradeOffer(Guid id, Guid user, Guid offeredCard, Guid wantedCard) : base(id)
        {
            UserID = user;
            OfferedCardID = offeredCard;
            WantedCardID = wantedCard;
        }
    }
}
