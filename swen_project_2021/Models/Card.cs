using System;
using System.Text.Json.Serialization;
using MTCG.DAL;

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
    /// With a card, the user can obtain it
    /// and fight with. It splits into multiple subcategories like the MonsterCard
    /// or the SpellCard
    /// </summary>
    [Serializable]
    [TableName("cards")]
    public class Card : DatabaseObject
    {
        /// <summary>
        /// The unique name of the card
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The damage of the card
        /// </summary>
        [JsonPropertyName("damage")]
        public int Damage { get; set; }

        /// <summary>
        /// The type of the card should be set in 
        /// the derived constructors
        /// </summary>
        [JsonPropertyName("card_type")]
        public CardType CardType { get; set; }

        /// <summary>
        /// The element of the card
        /// </summary>
        [JsonPropertyName("element")]
        public Element Element { get; set; }

        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        public Card(string name, string description, int damage, CardType cardType, Element element, Rarity rarity) : base(Guid.NewGuid())
        {
            CardType = cardType;
            Name = name;
            Description = description;
            Damage = damage;
            Element = element;
            Rarity = rarity;
        }

        public override bool Equals(object obj)
        {
            return obj is Card c &&
                c.Id == this.Id &&
                c.Name == this.Name &&
                c.Description == this.Description &&
                c.Rarity == this.Rarity &&
                c.CardType == this.CardType &&
                c.Damage == this.Damage &&
                c.Element == this.Element;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Damage, CardType, Element, Rarity, Description);
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
