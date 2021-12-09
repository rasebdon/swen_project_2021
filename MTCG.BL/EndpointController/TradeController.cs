using MTCG.BL.EndpointController.Requests;
using MTCG.BL.Http;
using MTCG.BL.Services;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Newtonsoft.Json;
using System.Net.Mime;

namespace MTCG.BL.EndpointController
{
    [HttpEndpoint("/trades")]
    public class TradeController : Controller
    {
        private readonly CardRepository _cardRepository;
        private readonly TradeRepository _tradeRepository;
        private readonly StackRepository _stackRepository;
        private readonly OfferRepository _offerRepository;
        private readonly AuthenticationService _authenticationService;
        private readonly ILog _log;

        public TradeController(
            AuthenticationService authenticationService,
            CardRepository cardRepository,
            StackRepository stackRepository,
            TradeRepository tradeRepository,
            OfferRepository offerRepository,
            ILog log)
        {
            _authenticationService = authenticationService;
            _tradeRepository = tradeRepository;
            _offerRepository = offerRepository;
            _stackRepository = stackRepository;
            _cardRepository = cardRepository;
            _log = log;
        }

        [HttpPost]
        public HttpResponse PlaceOffer(HttpRequest request)
        {
            try
            {
                // Authenticate via Token
                User? user = _authenticationService.Authenticate(request.Authorization);

                if (user == null)
                    return new HttpResponse(HttpStatusCode.Forbidden);

                // Parse request body
                PostTradeOfferRequestBody? requestBody;
                try
                {
                    requestBody = JsonConvert.DeserializeObject<PostTradeOfferRequestBody>(request.RequestBody);

                    if (requestBody == null)
                        throw new ArgumentNullException();
                }
                catch (Exception)
                {
                    return new HttpResponse(HttpStatusCode.BadRequest);
                }

                // Try to find offered card in users stack
                CardInstance? offeredCard = _stackRepository.GetById(user.ID)?.Find(c => c.ID == requestBody.OfferedCardId);

                if(offeredCard == null)
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Find wanted card in card database
                Card? wantedCard = _cardRepository.GetById(requestBody.WantedCardId);

                if (wantedCard == null)
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Create trade offer and return it
                TradeOffer offer = new(Guid.NewGuid(), user.ID, offeredCard.ID, wantedCard.ID);

                try
                {
                    if (!_offerRepository.Insert(offer))
                        throw new Exception();
                }
                catch(Exception ex)
                {
                    _log.WriteLine(ex.ToString());
                    return new HttpResponse(HttpStatusCode.BadRequest);
                }

                return new HttpResponse(
                    offer.ToJson(),
                    HttpStatusCode.Created,
                    MediaTypeNames.Application.Json);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponse ListAllOffers(HttpRequest request)
        {
            try
            {
                List<TradeOffer> offers = new(_offerRepository.GetAll());

                return new HttpResponse(
                    JsonConvert.SerializeObject(offers.ToArray(), Formatting.Indented),
                    HttpStatusCode.OK,
                    MediaTypeNames.Application.Json);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [HttpEndpointArgument]
        public HttpResponse DeleteOffer(HttpRequest request)
        {
            try
            {
                // Authenticate via Token
                User? user = _authenticationService.Authenticate(request.Authorization);

                if (user == null)
                    return new HttpResponse(HttpStatusCode.Forbidden);

                // Try to parse request argument
                if (!Guid.TryParse(request.Argument, out Guid offerId))
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Try to find offered card in users stack
                List<CardInstance>? stack = _stackRepository.GetById(user.ID);
                CardInstance? offeredCard = stack?.Find(c => c.ID == offerId);

                if (offeredCard == null || stack == null)
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Try to delete offer
                try
                {
                    if (!_offerRepository.Delete(offerId))
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    _log.WriteLine(ex.ToString());
                    return new HttpResponse(HttpStatusCode.BadRequest);
                }

                return new HttpResponse(
                    JsonConvert.SerializeObject(stack.ToArray()),
                    HttpStatusCode.Created,
                    MediaTypeNames.Application.Json);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [HttpEndpointArgument]
        public HttpResponse UpdateOffer(HttpRequest request)
        {
            try
            {
                // Authenticate via Token
                User? user = _authenticationService.Authenticate(request.Authorization);

                if (user == null)
                    return new HttpResponse(HttpStatusCode.Forbidden);

                // Parse request body
                PostTradeOfferRequestBody? requestBody;
                try
                {
                    requestBody = JsonConvert.DeserializeObject<PostTradeOfferRequestBody>(request.RequestBody);

                    if (requestBody == null)
                        throw new ArgumentNullException();
                }
                catch (Exception)
                {
                    return new HttpResponse(HttpStatusCode.BadRequest);
                }

                // Try to parse request argument
                if (!Guid.TryParse(request.Argument, out Guid offerId))
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Try to find offered card in users stack
                CardInstance? offeredCard = _stackRepository.GetById(user.ID)?.Find(c => c.ID == requestBody.OfferedCardId);

                if (offeredCard == null)
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Find wanted card in card database
                Card? wantedCard = _cardRepository.GetById(requestBody.WantedCardId);

                if (wantedCard == null)
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Update trade offer and return it
                TradeOffer offer = new(offerId, user.ID, offeredCard.ID, wantedCard.ID);
                try
                {
                    if (!_offerRepository.Update(offer))
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    _log.WriteLine(ex.ToString());
                    return new HttpResponse(HttpStatusCode.BadRequest);
                }

                return new HttpResponse(
                    offer.ToJson(),
                    HttpStatusCode.Created,
                    MediaTypeNames.Application.Json);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [HttpEndpointArgument]
        public HttpResponse AcceptOffer(HttpRequest request)
        {
            try
            {
                // Authenticate via Token
                User? user = _authenticationService.Authenticate(request.Authorization);

                if (user == null)
                    return new HttpResponse(HttpStatusCode.Forbidden);

                // Try to parse request argument
                if (!Guid.TryParse(request.Argument, out Guid offerId))
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Try to get offer from offerId
                TradeOffer? offer = _offerRepository.GetById(offerId);
                // Null check and check that user cannot trade with itself
                if(offer == null || offer.UserID == user.ID)
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Try to find offered card in users stack
                List<CardInstance>? stack = _stackRepository.GetById(user.ID);
                CardInstance? wantedCard = stack?.Find(c => c.CardID == offer.WantedCardID);

                if (wantedCard == null)
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Create trade
                Trade trade = new(
                    Guid.NewGuid(),
                    offer.UserID,
                    user.ID,
                    offer.OfferedCardInstanceId,
                    wantedCard.ID);

                // Insert trade and delete offer
                try
                {
                    // Insert trade (automatically switches cards)
                    if (!_tradeRepository.Insert(trade))
                        throw new Exception("Fatal error while executing trade transaction!");

                    // Delete offer
                    if (!_offerRepository.Delete(offer.ID))
                        throw new Exception("Fatal error while deleting offer!");
                }
                catch (Exception ex)
                {
                    _log.WriteLine(ex.ToString());
                    return new HttpResponse(HttpStatusCode.BadRequest);
                }

                stack = _stackRepository.GetById(user.ID);

                // Return users updated stack
                return new HttpResponse(
                    JsonConvert.SerializeObject(stack?.ToArray()),
                    HttpStatusCode.Created,
                    MediaTypeNames.Application.Json);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString());
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
