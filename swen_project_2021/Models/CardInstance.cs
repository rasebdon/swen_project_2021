using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    /// <summary>
    /// The instance of an abstract representation of a card
    /// <br></br>
    /// Has a unique id
    /// </summary>
    class CardInstance : DataObject
    {
        /// <summary>
        /// The id of the card that this card is the instance of
        /// </summary>
        public Guid CardID { get; }

        public CardInstance(OrderedDictionary row) : base(row)
        {
            CardID = Guid.Parse(row["cardID"].ToString());
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
