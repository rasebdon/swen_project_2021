using System.Text.Json.Serialization;

namespace MTCG.Models
{
    public enum CardType
    {
        Monster,
        Spell
    }

    public enum Element
    {
        Normal,
        Fire,
        Water
    }

    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public enum Race
    {
        None,
        Human,
        Elf,
        Orc,
        Troll,
        Draconid,
        Undead,
        Pirate,
        Beast,
        FireElf,
        Kraken,
        Knight,
        Mage,
        Goblin
    }

    /// <summary>
    /// A card is the abstract representation of the cards, the user can obtain
    /// and fight with. It splits into multiple subcategories like the MonsterCard
    /// or the SpellCard
    /// </summary>
    [Serializable]
    public class Card : DataObject
    {
        /// <summary>
        /// The unique name of the card
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The damage of the card
        /// </summary>
        public int Damage { get; }

        /// <summary>
        /// The type of the card should be set in 
        /// the derived constructors
        /// </summary>
        public CardType CardType { get; }
        /// <summary>
        /// The element of the card
        /// </summary>
        public Element Element { get; }
        public Race Race { get; }

        public Rarity Rarity { get; }

        public string Description { get; }

        [JsonConstructor]
        public Card(
            Guid id,
            string name,
            string description,
            int damage,
            CardType cardType,
            Element element,
            Race race,
            Rarity rarity) : base(id)
        {
            CardType = cardType;
            Name = name;
            Description = description;
            Damage = damage;
            Element = element;
            Race = race;
            Rarity = rarity;
        }


        public override bool Equals(object? obj)
        {
            return obj is Card c &&
                c.ID == this.ID &&
                c.Name == this.Name &&
                c.Description == this.Description &&
                c.Rarity == this.Rarity &&
                c.CardType == this.CardType &&
                c.Damage == this.Damage &&
                c.Element == this.Element;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name, Damage, CardType, Element, Rarity, Description);
        }

        public void SetID(Guid guid)
        {
            if (guid != Guid.Empty)
                ID = guid;
        }
    }
}
