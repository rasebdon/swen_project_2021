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

        [HttpPost]
        public HttpResponse CreateCard(HttpRequest request)
        {
            try
            {
                if(request.Authorization?.Token != "admin-mtcgToken")
                    return new HttpResponse(HttpStatusCode.Forbidden);

                // Try to add new card to game
                Card? card = JsonConvert.DeserializeObject<Card>(request.RequestBody);

                if(card != null)
                {
                    card.ID = Guid.NewGuid();
                    if(_cardRepository.Insert(card))
                    {
                        return new HttpResponse(
                            JsonConvert.SerializeObject(card, Formatting.Indented),
                            HttpStatusCode.Created,
                            MediaTypeNames.Application.Json);
                    }
                }
                return new HttpResponse(HttpStatusCode.BadRequest);
            }
            catch(Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
