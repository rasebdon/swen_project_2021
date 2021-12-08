using MTCG.BL.Http;
using MTCG.BL.Services;
using MTCG.DAL.Repositories;
using MTCG.Models;
using System.Collections.Concurrent;

namespace MTCG.BL.EndpointController
{
    [HttpEndpoint("/battles")]
    public class BattleController : Controller, IHttpPost
    {
        public ConcurrentQueue<Tuple<Deck, HttpRequest>> MatchQueue { get; }

        private readonly ILog _log;
        private readonly AuthenticationService _authentication;
        private readonly DeckRepository _deckRepository;

        public BattleController(AuthenticationService authentication, DeckRepository deckRepository, ILog log)
        {
            MatchQueue = new();
            _authentication = authentication;
            _deckRepository = deckRepository;
            _log = log;
        }

        [HttpPost]
        public HttpResponse Post(HttpRequest request)
        {
            try
            {
                // Auth user
                User? user = _authentication.Authenticate(request.Authorization);

                if (user != null)
                {
                    // Get main deck of user
                    Deck? deck = _deckRepository.GetUserDecks(user.ID).Find(d => d.MainDeck == true);

                    if (deck != null)
                    {
                        MatchQueue.Enqueue(new Tuple<Deck, HttpRequest>(deck, request));

                        _log.WriteLine($"{MatchQueue.Count} Battles in queue!");

                        return null; // Needs to hold connection
                    }
                }
                return new HttpResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                _log.WriteLine(e.ToString(), OutputFormat.Error);
                return new HttpResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
