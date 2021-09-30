using Newtonsoft.Json;
using System;
using System.Collections.Specialized;

namespace MTCG.Models
{
    public enum Race
    {
        Human,
        Elf,
        Orc,
        Troll,
        Draconid,
        Undead,
        Pirate,
        Beast,
        FireElf,
        Kraken,
        Knight,
        Mage,
        Goblin
    }

    [Serializable]
    public class MonsterCard : Card
    {
        /// <summary>
        /// The monsters "type"/race
        /// </summary>
        public Race Race { get; }

        public MonsterCard(OrderedDictionary row) : base(row)
        {
            Race = (Race)(int)row["race"];
        }

        [JsonConstructor]
        public MonsterCard(string name, string description, int damage, Element element, Rarity rarity, Race race) : base(name, description, damage, CardType.Monster, element, rarity)
        {
            Race = race;
        }

        public override bool Equals(object obj)
        {
            return obj is MonsterCard card &&
                   base.Equals(obj) &&
                   Race == card.Race;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Race);
        }
    }
}
