using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class MonsterCard : Card
    {
        public MonsterCard(OrderedDictionary row) : base(row)
        {
        }

        public MonsterCard(string name, string description, int damage, Element element, Rarity rarity) : base(name, description, damage, CardType.Monster, element, rarity)
        {

        }

        
    }
}
