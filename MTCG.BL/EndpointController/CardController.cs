using MTCG.BL.Http;
using MTCG.BL.Services;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Newtonsoft.Json;
using System.Net.Mime;

namespace MTCG.BL.EndpointController
{
    [HttpEndpoint("/cards")]
    public class CardController : Controller, IHttpGet
    {
        private readonly AuthenticationService _authenticationService;
        private readonly CardRepository _cardRepository;
        private readonly StackRepository _stackRepository;
        private readonly ILog _log;

        public CardController(
            AuthenticationService authenticationService,
            CardRepository cardRepository,
            StackRepository stackRepository, ILog log)
        {
            _authenticationService = authenticationService;
            _cardRepository = cardRepository;
            _stackRepository = stackRepository;
            _log = log;
        }

        [HttpGet]
        public HttpResponse Get(HttpRequest request)
        {
            try
            {
                // Authenticate user
                User? user = _authenticationService.Authenticate(request.Authorization);

                if (user != null)
                {
                    List<CardInstance>? stack = _stackRepository.GetById(user.ID);

                    if (stack != null)
                    {
                        return new HttpResponse(
                        JsonConvert.SerializeObject(stack, Formatting.Indented),
                        HttpStatusCode.OK,
                        MediaTypeNames.Application.Json);
                    }
                }
                return new HttpResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString(), OutputFormat.Error);
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }

        ///// <summary>
        ///// Get a list of cards that are related to the given card instances
        ///// </summary>
        ///// <param name="cards"></param>
        ///// <returns></returns>
        //public List<Card> GetCards(List<CardInstance> cardInstances)
        //{
        //    List<Card> cards = new();
        //    NpgsqlCommand cmd = new("SELECT * FROM cards WHERE id=@card_id;");

        //    for (int i = 0; i < cardInstances.Count; i++)
        //    {
        //        cmd.Parameters.AddWithValue("card_id", cardInstances[i].CardID);
        //        var row = Database.Instance.SelectSingle(cmd);
        //        cmd.Parameters.Clear();

        //        cards.Add(Card.ParseFromDatabase(row));
        //    }
        //    return cards;
        //}
        // Serialization
        //public string GetDetailedCardsJson(List<CardInstance> cards)
        //{
        //    CharStream s = new();
        //    s.Write("{ \"Cards\": [");

        //    for (int i = 0; i < cards.Count; i++)
        //    {
        //        s.Write(GetDetailedCardJson(cards[i]));
        //        if (i < cards.Count - 1)
        //            s.Write(",");
        //    }

        //    s.Write("]}");

        //    return s.ToString();
        //}
        //public string GetDetailedCardJson(CardInstance cardInstance)
        //{
        //    CharStream s = new();
        //    s.Write($"{{\"CardInstanceID\":\"{cardInstance.ID}\",");

        //    Card card = Select(cardInstance.CardID);

        //    s.Write($"\"CardID\":\"{card.ID}\",");
        //    s.Write($"\"Name\":\"{card.Name}\",");
        //    s.Write($"\"Description\":\"{card.Description}\",");
        //    s.Write($"\"CardType\":\"{card.CardType}\",");
        //    s.Write($"\"Damage\":\"{card.Damage}\",");
        //    s.Write($"\"Element\":\"{card.Element}\",");
        //    s.Write($"\"Rarity\":\"{card.Rarity}\"{ (card.CardType == CardType.Monster ? "," : "") }");
        //    if(card.CardType == CardType.Monster)
        //        s.Write($"\"Race\":\"{(card as MonsterCard).Race}\",");
        //    s.Write("}");
        //    return s.ToString();
        //}
        //public string GetDetailedDeckJson(Deck deck)
        //{
        //    CharStream s = new();
        //    s.Write("{");
        //    s.Write($"\"ID\":\"{deck.ID}\",");
        //    s.Write($"\"Name\":\"{deck.Name}\",");
        //    s.Write($"\"UserID\":\"{deck.UserID}\",");
        //    s.Write($"\"Cards\": [");

        //    for (int i = 0; i < deck.Cards.Count; i++)
        //    {
        //        s.Write(GetDetailedCardJson(deck.Cards[i]));
        //        if (i < deck.Cards.Count - 1)
        //            s.Write(",");
        //    }

        //    s.Write("]}");
        //    return s.ToString();
        //}
        //public string GetDetailedDecksJson(List<Deck> decks)
        //{
        //    CharStream s = new();
        //    s.Write("{ \"Decks\": [");

        //    for (int i = 0; i < decks.Count; i++)
        //    {
        //        s.Write(GetDetailedDeckJson(decks[i]));
        //        if (i < decks.Count - 1)
        //            s.Write(",");
        //    }

        //    s.Write("]}");

        //    return s.ToString();
        //}
    }
}
