using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    /// <summary>
    /// A card is the abstract representation of the cards, the user can obtain
    /// and fight with. It splits into multiple subcategories like the MonsterCard
    /// or the SpellCard
    /// </summary>
    abstract class Card : IValueObject
    {
        /// <summary>
        /// The unique card id
        /// </summary>
        public uint ID { get; }
        /// <summary>
        /// The unique name of the card
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The damage of the card
        /// </summary>
        public int Damage { get; }

        protected Card(uint id, string name, int damage)
        {
            ID = id;
            Name = name;
            Damage = damage;
        }
    }
}
