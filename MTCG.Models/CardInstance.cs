using Newtonsoft.Json;

namespace MTCG.Models
{
    /// <summary>
    /// The instance of an abstract representation of a card
    /// <br></br>
    /// Has a unique id
    /// </summary>
    public class CardInstance : Card
    {
        /// <summary>
        /// The id of the card that this card is the instance of
        /// </summary>
        public Guid CardID { get; set; }

        /// <summary>
        /// Creates an instance of a card
        /// </summary>
        /// <param name="card">The card that this card is the instance of</param>
        public CardInstance(Guid id, Card card)
            : base(id, card.Name, card.Description, card.Damage, card.CardType, card.Element, card.Race, card.Rarity)
        {
            CardID = card.ID;
        }

        [JsonConstructor]
        public CardInstance(Guid id, Guid cardId, string name, string description, int damage, CardType cardType, Element element, Race race, Rarity rarity) : base(id, name, description, damage, cardType, element, race, rarity)
        {
            CardID = cardId;
        }
    }
}
