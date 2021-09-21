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
        public Card[] Cards { get; }

        public Package(uint id, Card[] cards) : base(id)
        {
            // Check for package card count
            if (cards == null || cards.Length != 5)
                throw new ArgumentException("A package needs exactly 5 cards!");

            Cards = cards;
        }

        public Package(OrderedDictionary row) : base(row)
        {
            Cards = System.Text.Json.JsonSerializer.Deserialize<Card[]>(row["cards"].ToString());
        }
    }
}
