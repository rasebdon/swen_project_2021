namespace MTCG.Models
{
    public class Deck : DataObject
    {
        public const ushort DeckSize = 4;
        public string Name { get; set; }
        public Guid UserID { get; }
        public bool MainDeck { get; set; }
        public List<CardInstance> Cards { get; }

        public Deck(Guid id, string name, Guid userId, bool mainDeck, CardInstance[] cards) : base(id)
        {
            Name = name;
            UserID = userId;
            MainDeck = mainDeck;
            Cards = new(cards);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Deck deck)
            {
                if (deck.Cards.Count != Cards.Count)
                    return false;

                foreach (Card card in Cards)
                {
                    if (deck.Cards.Where(c => c.Equals(card)) == null)
                    {
                        return false;
                    }
                }

                return ID.Equals(deck.ID) &&
                       Name == deck.Name &&
                       UserID == deck.UserID &&
                       MainDeck == deck.MainDeck;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name, UserID, MainDeck, Cards);
        }
    }
}
