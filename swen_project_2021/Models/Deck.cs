using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MTCG.Models
{
    class Deck : DataObject
    {
        public List<Card> Cards { get; private set; }

        public Deck(uint id, List<Card> cards) : base(id)
        {
            Cards = cards;
        }

        public Deck(OrderedDictionary row) : base(row)
        {
            // Parse cards in deck
            throw new NotImplementedException();
        }
    }
}