using System.Text.Json.Serialization;

namespace MTCG.Models
{
    public class Package : DataObject
    {
        public const ushort DrawnCardsAmount = 5;

        public string Name { get; }
        public string Description { get; }
        public ushort Cost { get; }
        public IEnumerable<Card> Cards { get; }

        [JsonConstructor]
        public Package(Guid id, string name, string description, ushort cost, IEnumerable<Card> cards) : base(id)
        {
            // Check for package card count
            if (cards == null || cards.Count() == 0)
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

        public override bool Equals(object? obj)
        {
            if (obj is Package package)
            {
                if (package.Cards.Count() != Cards.Count())
                    return false;

                foreach (Card card in Cards)
                {
                    if (package.Cards.Where(c => c.Equals(card)) == null)
                    {
                        return false;
                    }
                }

                return ID.Equals(package.ID) &&
                       Name == package.Name &&
                       Description == package.Description &&
                       Cost == package.Cost;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name, Description, Cost, Cards);
        }
    }
}
