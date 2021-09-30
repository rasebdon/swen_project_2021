using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    /// <summary>
    /// A card is the abstract representation of the cards, the user can obtain
    /// and fight with. It splits into multiple subcategories like the MonsterCard
    /// or the SpellCard
    /// </summary>
    public abstract class Card : DataObject
    {
        /// <summary>
        /// The unique name of the card
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The damage of the card
        /// </summary>
        public int Damage { get; }

        public static Card ParseFromDatabase(OrderedDictionary row)
        {
            CardType t = Enum.Parse<CardType>(row["type"].ToString());

            switch (t)
            {
                case CardType.Monster:
                    return new MonsterCard(row);
                case CardType.Spell:
                    return new MonsterCard(row);
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// The type of the card should be set in 
        /// the derived constructors
        /// </summary>
        public CardType CardType { get; }
        /// <summary>
        /// The element of the card
        /// </summary>
        public Element Element { get; }

        public Rarity Rarity { get; }

        public string Description { get; }

        protected Card(OrderedDictionary row) : base(row)
        {
            Name = row["name"].ToString();
            Description = row["description"].ToString();
            CardType = (CardType)(int)row["type"];
            Damage = (int)row["damage"];
            Element = (Element)(int)row["element"];
            Rarity = (Rarity)(int)row["rarity"];
        }
        protected Card(string name, string description, int damage, CardType cardType, Element element, Rarity rarity) : base()
        {
            CardType = cardType;
            Name = name;
            Description = description;
            Damage = damage;
            Element = element;
            Rarity = rarity;
        }
    }
}
