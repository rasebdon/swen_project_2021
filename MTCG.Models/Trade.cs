namespace MTCG.Models
{
    public class Trade : DataObject
    {
        public Guid OfferUserId { get; }
        public Guid AcceptUserId { get; }
        public Guid OfferedCardInstanceId { get; }
        public Guid WantedCardInstanceId { get; }

        public Trade(Guid id, Guid offerUserId, Guid acceptUserId, Guid offeredCardId, Guid wantedCardId) : base(id)
        {
            OfferUserId = offerUserId;
            AcceptUserId = acceptUserId;
            OfferedCardInstanceId = offeredCardId;
            WantedCardInstanceId = wantedCardId;
        }

    }
}
