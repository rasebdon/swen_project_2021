using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Package : DataObject
    {
        public const ushort DrawnCardsAmount = 5;

        public string Name { get; }
        public string Description { get; }
        public ushort Cost { get; }
        public List<Card> Cards { get; }

        public Package(List<Card> cards, ushort cost) : base()
        {
            // Check for package card count
            if (cards == null || cards.Count == 0)
                throw new ArgumentException();

            Cards = cards;
        }

        public Package(OrderedDictionary packageRow, OrderedDictionary[] packageCardsRows) : base(packageRow)
        {
            Name = packageRow["name"].ToString();
            Description = packageRow["description"].ToString();
            Cost = (ushort)packageRow["cost"];

            // Add cards
            for (int i = 0; i < packageCardsRows.Length; i++)
            {
                Cards.Add(Card.ParseFromDatabase(packageCardsRows[i]));
            }
        }
    }
}
