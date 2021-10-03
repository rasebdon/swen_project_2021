using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Controller
{
    public class BattleController : Singleton<BattleController>
    {
        public const int MaxPlayedRounds = 100;

        public void Battle(Deck deck1, Deck deck2)
        {
            // Init battle
            for (int i = 0; i < MaxPlayedRounds; i++)
            {

            }
        }
    }
}
