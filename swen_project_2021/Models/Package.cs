using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MTCG.Models
{
    public class Package : DataObject
    {
        public const ushort DrawnCardsAmount = 5;

        public string Name { get; }
        public string Description { get; }
        public ushort Cost { get; }
        public List<Card> Cards { get; }

        [JsonConstructor]
        public Package(string name, string description, ushort cost, List<Card> cards) : base()
        {
            // Check for package card count
            if (cards == null || cards.Count == 0)
                throw new ArgumentException();

            Name = name;
            Description = description;
            Cost = cost;
            Cards = cards;
        }

        public void SetID(Guid guid)
        {
            if (guid != Guid.Empty)
                ID = guid;
        }

        public Package(OrderedDictionary packageRow, OrderedDictionary[] packageCardsRows) : base(packageRow)
        {
            Name = packageRow["name"].ToString();
            Description = packageRow["description"].ToString();
            Cost = (ushort)(int)packageRow["cost"];

            Cards = new();
            // Add cards
            for (int i = 0; i < packageCardsRows.Length; i++)
            {
                Cards.Add(Card.ParseFromDatabase(packageCardsRows[i]));
            }
        }
    }
}
