using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    class SpellCard : Card
    {
        public SpellCard(uint id, string name, int damage, Element element, Rarity rarity) : base(id, name, damage, CardType.Spell, element, rarity)
        {

        }
    }
}
