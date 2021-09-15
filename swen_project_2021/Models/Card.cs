using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        protected Card(OrderedDictionary row) : base(row)
        {
            Name = row["name"].ToString();
            Damage = (int)row["damage"];
        }
        protected Card(uint id, string name, int damage) : base(id)
        {
            Name = name;
            Damage = damage;
        }
    }
}
