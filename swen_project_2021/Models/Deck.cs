using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Deck : DataObject
    {
        public const ushort DeckSize = 4;

        public Deck(OrderedDictionary deckInformationRow, OrderedDictionary[] cardRows) : base(deckInformationRow)
        {
            UserID = (Guid)deckInformationRow["user_id"];
            Name = deckInformationRow["name"].ToString();

            Cards = new();
            // Add cards
            for (int i = 0; i < cardRows.Length; i++)
            {
                Cards.Add(new CardInstance(cardRows[i]));
            }
        }

        public Deck(string name, Guid userID, List<CardInstance> cards)
        {
            // Check for package card count
            if (cards == null || cards.Count != DeckSize)
                throw new ArgumentException($"The cards of a deck must be { DeckSize }"!);

            Name = name;
            UserID = userID;
            Cards = cards;
        }

        public string Name { get; }
        public Guid UserID { get; }
        public List<CardInstance> Cards { get; }


    }
}
