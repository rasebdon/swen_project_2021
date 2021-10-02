using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class TradeOffer : DataObject
    {
        public Guid UserID { get; }
        public Guid OfferedCardID { get; }
        public Guid WantedCardID { get; }

        public TradeOffer(User user, CardInstance offeredCard, Card wantedCard) : base()
        {
            UserID = user.ID;
            OfferedCardID = offeredCard.ID;
            WantedCardID = wantedCard.ID;
        }

        public TradeOffer(OrderedDictionary row) : base(row)
        {
            UserID = (Guid)row["user_id"];
            OfferedCardID = (Guid)row["offered_card_id"];
            WantedCardID = (Guid)row["wanted_card_id"];
        }
    }
}
