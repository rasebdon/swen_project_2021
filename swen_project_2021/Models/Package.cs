using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json.Serialization;
using MTCG.DAL;

namespace MTCG.Models
{
    [Serializable]
    [TableName("packages")]
    public class Package : DatabaseObject
    {
        public const ushort DrawnCardsAmount = 5;

        [JsonPropertyName("name")]
        public string Name { get; }
        [JsonPropertyName("description")]
        public string Description { get; }
        [JsonPropertyName("cost")]
        public ushort Cost { get; }

        [JsonPropertyName("cards")]
        public List<Card> Cards { get; }

        [JsonConstructor]
        public Package(Guid id, string name, string description, ushort cost, List<Card> cards) : base(id)
        {
            // Check for package card count
            if (cards == null || cards.Count == 0)
                throw new ArgumentException("Cards cannot be an empty list!");

            Name = name;
            Description = description;
            Cost = cost;
            Cards = cards;
        }

        public Package(string name, string description, ushort cost, List<Card> cards)
            : this(Guid.NewGuid(), name, description, cost, cards)
        { }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
