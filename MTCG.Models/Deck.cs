namespace MTCG.Models
{
    public class Deck : DataObject
    {
        public const ushort DeckSize = 4;
        public string Name { get; }
        public Guid UserID { get; }
        public bool MainDeck { get; }
        public List<CardInstance> Cards { get; }

        public Deck(Guid id, string name, Guid userId, bool mainDeck, CardInstance[] cards) : base(id)
        {
            Name = name;
            UserID = userId;
            MainDeck = mainDeck;
            Cards = new(cards);
        }
    }
}
