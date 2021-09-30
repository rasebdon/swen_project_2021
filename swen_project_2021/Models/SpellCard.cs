using System.Collections.Specialized;

namespace MTCG.Models
{
    public class SpellCard : Card
    {
        public SpellCard(OrderedDictionary row) : base(row)
        {
        }

        public SpellCard(string name, string description, int damage, Element element, Rarity rarity) : base(name, description, damage, CardType.Spell, element, rarity)
        {

        }
    }
}
