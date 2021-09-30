using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    class SpellCard : Card
    {
        public SpellCard(OrderedDictionary row) : base(row)
        {
        }

        public SpellCard(string name, string description, int damage, Element element, Rarity rarity) : base(name, description, damage, CardType.Spell, element, rarity)
        {

        }
    }
}
