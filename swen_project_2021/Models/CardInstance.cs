using MTCG.DAL;
using System;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace MTCG.Models
{
    /// <summary>
    /// The instance of an abstract representation of a card
    /// <br></br>
    /// Has a unique id
    /// </summary>
    [Serializable]
    [TableName("card_instances")]
    public class CardInstance : DatabaseObject
    {
        /// <summary>
        /// The id of the card that this card is the instance of
        /// </summary>
        [JsonPropertyName("card_id")]
        public Guid CardID { get; set; }

        /// <summary>
        /// Creates an instance of a card
        /// </summary>
        /// <param name="card">The card that this card is the instance of</param>
        public CardInstance(Card card) : base(Guid.NewGuid())
        {
            CardID = card.Id;
        }

        [JsonConstructor]
        public CardInstance(Guid id, Guid cardId) : base(id)
        {
            CardID = cardId;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
