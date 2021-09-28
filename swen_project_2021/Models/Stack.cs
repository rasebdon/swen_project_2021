﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MTCG.Models
{
    class Stack : DataObject
    {
        public User User { get; private set; }
        public List<Card> Cards { get; private set; }

        public Stack(uint id, List<Card> cards, User user) : base(id)
        {
            Cards = cards;
            User = user;
        }

        public Stack(OrderedDictionary row) : base(row)
        {
            // Parse cards in stack
            throw new NotImplementedException();
        }
    }
}