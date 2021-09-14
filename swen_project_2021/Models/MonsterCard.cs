using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    class MonsterCard : Card
    {
        public MonsterCard(uint id, string name, int damage) : base(id, name, damage)
        {
        }
    }
}
