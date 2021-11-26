using System.Text.Json.Serialization;

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
        public Package(Guid id, string name, string description, ushort cost, List<Card> cards) : base(id)
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
    }
}
