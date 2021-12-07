using MTCG.BL.Http;
using MTCG.BL.Services;
using MTCG.DAL.Repositories;
using MTCG.Models;
using Newtonsoft.Json;
using System.Net.Mime;

namespace MTCG.BL.EndpointController
{
    /// <summary>
    /// This controller manages the package related functions
    /// </summary>
    [HttpEndpoint("/packages")]
    public class PackageController : Controller, IHttpPost
    {
        private readonly ILog _log;
        private readonly UserRepository _userRepository;
        private readonly PackageRepository _packageRepository;
        private readonly CardInstanceRepository _cardInstanceRepository;
        private readonly AuthenticationService _authenticationService;

        public PackageController(
            AuthenticationService authenticationService,
            UserRepository userRepository,
            PackageRepository packageRepository,
            CardInstanceRepository cardInstanceRepository,
            ILog log)
        {
            _authenticationService = authenticationService;
            _cardInstanceRepository = cardInstanceRepository;
            _packageRepository = packageRepository;
            _userRepository = userRepository;
            _log = log;
        }

        // Helper Methods
        private List<Card>? GetCardsWithRarity(IEnumerable<Card> _cards, Rarity rarity)
        {
            // Get the cards with the correct rarity
            List<Card> correctRarity = new List<Card>(_cards).FindAll(c => c.Rarity == rarity);
            if (correctRarity.Count == 0)
                return null;
            return correctRarity;
        }
        private Card? GetRandomCardWithRarity(IEnumerable<Card> cards, Rarity rarity)
        {
            var c = GetCardsWithRarity(cards, rarity);
            if (c == null)
                return null;
            return c[new Random().Next(0, c.Count)];
        }
        private CardInstance? GetRandomCardInstanceWithRarity(IEnumerable<Card> cards, Rarity rarity)
        {
            var c = GetRandomCardWithRarity(cards, rarity);
            if (c == null)
                return null;
            return new CardInstance(Guid.NewGuid(), c);
        }

        private List<CardInstance>? BuyPackage(User user, Package package)
        {
            // Open the package
            List<CardInstance>? drawnCards = new();

            try
            {
                // Draw cards (which card will be drawn depends on its rarity)
                for (int i = 0; i < Package.DrawnCardsAmount; i++)
                {
                    CardInstance? card = null;
                    // Check if a card was drawn, if not, repeat
                    while (card == null)
                    {
                        // Roll
                        int roll = new Random().Next(0, 100);
                        if (roll > 99)
                        {
                            card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Legendary);
                        }
                        else if (roll > 90)
                        {
                            card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Epic);
                        }
                        else if (roll > 75)
                        {
                            card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Rare);
                        }
                        else
                        {
                            card = GetRandomCardInstanceWithRarity(package.Cards, Rarity.Common);
                        }
                    }
                    if (card != null)
                        drawnCards.Add(card);
                }

                if (drawnCards.Count != Package.DrawnCardsAmount)
                    throw new Exception("Fatal Error, Invalid card amount drawn");

                // Add drawn cards to database
                if (!_cardInstanceRepository.Insert(drawnCards))
                    throw new Exception("Fatal Error occured while Inserting CardInstances!");

            }
            catch (Exception ex)
            {
                _log.WriteLine("An error occured while drawing cards!", OutputFormat.Error);
                _log.WriteLine(ex.ToString(), OutputFormat.Error);
                return null;
            }

            if (drawnCards != null)
            {
                // Add the cards to the user
                if (!_cardInstanceRepository.AddCardInstancesToUser(user, drawnCards))
                    return null;

                // Update user money
                user.Coins -= package.Cost;
                _userRepository.Update(user);
            }

            return drawnCards;
        }

        [HttpPost]
        public HttpResponse Post(HttpRequest request)
        {
            Package? package = null;

            try
            {
                // Check if authorization is admin-token
                if (request.Authorization?.Token != "admin-mtcgToken")
                    return new HttpResponse(HttpStatusCode.Forbidden);

                // Parse package
                package = JsonConvert.DeserializeObject<Package>(request.RequestBody);

                try
                {
                    if (package == null)
                        throw new ArgumentNullException("package", "Invalid JSON provided!");
                }
                catch (Exception)
                {
                    return new HttpResponse(HttpStatusCode.BadRequest);
                }
                
                // Generate new id
                package.ID = Guid.NewGuid();

                // Insert package
                if (!_packageRepository.Insert(package))
                    throw new Exception("Error inserting package into db!");

                return new HttpResponse(package.ToJson(), HttpStatusCode.Created, "application/json");
            }
            catch (Exception ex)
            {
                if (package != null)
                    _packageRepository.Delete(package.ID);
                _log.WriteLine(ex.ToString(), OutputFormat.Error);

                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [HttpEndpointArgument]
        [HttpEndpoint("/transactions/packages")]
        public HttpResponse BuyPackagePost(HttpRequest request)
        {
            try
            {
                // Get user via auth token
                User? user = _authenticationService.Authenticate(request.Authorization);

                if (user == null)
                    return new HttpResponse(HttpStatusCode.Forbidden);

                // Parse request body
                _log.WriteLine(request.Argument);

                if (!Guid.TryParse(request?.Argument ?? "", out Guid packageId))
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Get package via name
                Package? package = _packageRepository.GetById(packageId);

                if (package == null)
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Check if user has enough coins
                if (user.Coins < package.Cost)
                    return new HttpResponse(HttpStatusCode.BadRequest);

                // Buy package
                List<CardInstance>? drawnCards = new();
                if ((drawnCards = BuyPackage(user, package)) == null)
                    throw new Exception("Error buying package!");

                // Return drawn cards as json array
                return new HttpResponse(
                    JsonConvert.SerializeObject(drawnCards, Formatting.Indented),
                    HttpStatusCode.Created,
                    MediaTypeNames.Application.Json);
            }
            catch (Exception ex)
            {
                _log.WriteLine(ex.ToString(), OutputFormat.Error);
                return new HttpResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
