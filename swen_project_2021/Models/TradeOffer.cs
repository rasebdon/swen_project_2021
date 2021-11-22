using MTCG.DAL;
using System.Text.Json.Serialization;
using System;

namespace MTCG.Models
{
    [Serializable]
    [TableName("offers")]
    public class TradeOffer : DatabaseObject
    {
        [JsonPropertyName("user_id")]
        public Guid UserID { get; }
        [JsonPropertyName("offered_card_id")]
        public Guid OfferedCardID { get; }
        [JsonPropertyName("wanted_card_id")]
        public Guid WantedCardID { get; }

        public TradeOffer(User user, CardInstance offeredCard, Card wantedCard) : base(Guid.NewGuid())
        {
            UserID = user.Id;
            OfferedCardID = offeredCard.Id;
            WantedCardID = wantedCard.Id;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
