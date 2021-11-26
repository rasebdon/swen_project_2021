using MTCG.BL.Http;
using MTCG.BL.Services;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Reflection;

namespace MTCG.BL.EndpointController
{
    [HttpEndpoint("/decks")]
    public class DeckController : Controller, IHttpGet, IHttpPost, IHttpPut
    {
        private readonly ILog _log;
        private readonly DeckRepository _deckRepository;
        private readonly StackRepository _stackRepository;
        private readonly AuthenticationService _authenticationService;

        public DeckController(
            AuthenticationService authenticationService,
            DeckRepository deckRepository,
            StackRepository stackRepository,
            ILog log)
        {
            _log = log;
            _deckRepository = deckRepository;
            _stackRepository = stackRepository;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Returns all the users decks as json
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponse Get(HttpRequest request)
        {
            // Authenticate user
            User? user = _authenticationService.Authenticate(request.Authorization);

            try
            {
                if (user != null)
                {
                    List<Deck> deck = _deckRepository.GetUserDecks(user.ID);

                    return new HttpResponse(
                        JsonConvert.SerializeObject(deck.ToArray()),
                        HttpStatusCode.OK,
                        MediaTypeNames.Application.Json);
                }
                else return new HttpResponse(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString(), OutputFormat.Error);
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponse Post(HttpRequest request)
        {
            try
            {
                // Get user auth
                User? user = _authenticationService.Authenticate(request.Authorization);

                if (user != null)
                {
                    // Create deck
                    Deck? deck = JsonConvert.DeserializeObject<Deck>(request.RequestBody);
                    if (deck != null && Validate(deck) && _deckRepository.Insert(deck))
                    {
                        return new HttpResponse(HttpStatusCode.Created);
                    }
                    return new HttpResponse(HttpStatusCode.BadRequest);
                }
                return new HttpResponse(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString(), OutputFormat.Error);
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [HttpEndpointArgument]
        public HttpResponse Put(HttpRequest request)
        {
            try
            {
                User? user = _authenticationService.Authenticate(request.Authorization);

                if (user != null)
                {
                    // Parse deck from json
                    Deck? newDeck = JsonConvert.DeserializeObject<Deck>(request.RequestBody);

                    if (newDeck != null && Validate(newDeck))
                    {
                        // Try to retrieve id from path
                        //string? arg = (MethodBase.GetCurrentMethod()?
                        //    .GetCustomAttributes(typeof(HttpEndpointArgumentAttribute), true)[0]
                        //    as HttpEndpointArgumentAttribute)?.Argument;
                        string? arg = request.Argument;

                        if (arg != null)
                        {
                            // Get deck to update id from query
                            Deck? deckToUpdate = _deckRepository.GetById(Guid.Parse(arg));

                            if (deckToUpdate != null)
                            {
                                // Check if user is owner of deck
                                if (deckToUpdate.UserID != user.ID)
                                    return new HttpResponse(HttpStatusCode.Forbidden);

                                // Update deck
                                if (_deckRepository.Update(deckToUpdate, newDeck))
                                    return new HttpResponse(HttpStatusCode.Created);
                            }
                        }
                    }
                    return new HttpResponse(HttpStatusCode.BadRequest);
                }
                return new HttpResponse(HttpStatusCode.Forbidden);
            }
            catch (Exception e)
            {
                _log.WriteLine(e.ToString(), OutputFormat.Error);
                return new HttpResponse(HttpStatusCode.BadRequest);
            }
        }

        private bool Validate(Deck deck)
        {
            try
            {
                if (deck == null)
                    throw new NullReferenceException("Deck cannot be null!");

                if (deck.Cards.Count != Deck.DeckSize)
                    throw new ArgumentException("Invalid deck size of deck!");

                // Check if cards belong to the user
                List<CardInstance> cards = new();
                List<CardInstance>? stack = _stackRepository.GetById(deck.UserID);

                if (stack != null)
                {
                    foreach (CardInstance card in deck.Cards)
                    {
                        if (stack.Contains(card))
                        {
                            if (cards.Find(c => c.ID == card.ID) != null)
                                throw new ArgumentException("Each card can only be in a deck one time!");
                            cards.Add(card);
                        }
                    }
                }
                if (cards.Count != Deck.DeckSize)
                    throw new ArgumentException("Some cards in the deck do not belong to the user!");

                return true;
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return false;
            }
        }

    }
}
