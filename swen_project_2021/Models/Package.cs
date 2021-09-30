using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    class Package : DataObject
    {
        public const ushort PackageCardsLength = 5;

        public ushort Cost { get; }
        public Card[] Cards { get; }

        public Package(uint id, Card[] cards, ushort cost) : base(id)
        {
            // Check for package card count
            if (cards == null || cards.Length != PackageCardsLength)
                throw new ArgumentException("A package needs exactly 5 cards!");

            Cards = cards;

        }

        public Package(OrderedDictionary row) : base(row)
        {
            Cards = System.Text.Json.JsonSerializer.Deserialize<Card[]>(row["cards"].ToString());
        }
    }
}
