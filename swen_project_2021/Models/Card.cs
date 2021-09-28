using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    enum CardType
    {
        Monster,
        Spell
    }

    /// <summary>
    /// A card is the abstract representation of the cards, the user can obtain
    /// and fight with. It splits into multiple subcategories like the MonsterCard
    /// or the SpellCard
    /// </summary>
    abstract class Card : DataObject
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

        protected Card(OrderedDictionary row) : base(row)
        {
            CardType = Enum.Parse<CardType>(row["type"].ToString());
            Name = row["name"].ToString();
            Damage = (int)row["damage"];
        }
        protected Card(uint id, string name, int damage, CardType cardType) : base(id)
        {
            CardType = cardType;
            Name = name;
            Damage = damage;
        }
    }
}
