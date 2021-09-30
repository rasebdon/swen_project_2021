using System;
using System.Collections.Specialized;

namespace MTCG.Models
{
    /// <summary>
    /// The instance of an abstract representation of a card
    /// <br></br>
    /// Has a unique id
    /// </summary>
    public class CardInstance : DataObject
    {
        /// <summary>
        /// The id of the card that this card is the instance of
        /// </summary>
        public Guid CardID { get; }

        public CardInstance(OrderedDictionary row) : base(row)
        {
            CardID = Guid.Parse(row["card_id"].ToString());
        }

        /// <summary>
        /// Creates an instance of a card
        /// </summary>
        /// <param name="card">The card that this card is the instance of</param>
        public CardInstance(Card card) : base()
        {
            CardID = card.ID;
        }
    }
}
