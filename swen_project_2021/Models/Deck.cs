using MTCG.DAL;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace MTCG.Models
{
    [Serializable]
    [TableName("decks")]
    public class Deck : DatabaseObject
    {
        public const ushort DeckSize = 4;
        public string Name { get; }
        public Guid UserID { get; }

        public List<CardInstance> Cards { get; }
        public bool MainDeck { get; }

        [JsonConstructor]
        public Deck(string name, Guid userId, bool mainDeck, CardInstance[] cards) : base(Guid.NewGuid())
        {
            Name = name;
            UserID = userId;
            MainDeck = mainDeck;
            Cards = new(cards);
        }

        public Deck(string name, Guid userID, List<CardInstance> cards, bool mainDeck) : base(Guid.NewGuid())
        {
            // Check for package card count
            if (cards == null || cards.Count != DeckSize)
                throw new ArgumentException($"The cards of a deck must be { DeckSize }"!);

            MainDeck = mainDeck;
            Name = name;
            UserID = userID;
            Cards = cards;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
