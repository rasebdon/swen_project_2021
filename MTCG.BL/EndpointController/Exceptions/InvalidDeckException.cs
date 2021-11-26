using MTCG.Models;

namespace MTCG.BL.EndpointController.Exceptions
{
    [Serializable]
    internal class InvalidDeckException : Exception
    {
        public InvalidDeckException(Deck deck)
            : base($"Validation of deck {deck.ID} for user {deck.UserID} failed!") { }
    }
}