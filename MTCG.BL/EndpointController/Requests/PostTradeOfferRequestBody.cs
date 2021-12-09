using Newtonsoft.Json;

namespace MTCG.BL.EndpointController.Requests
{
    public class PostTradeOfferRequestBody : JsonRequestBody<PostTradeOfferRequestBody>
    {
        [JsonIgnore]
        public Guid OfferedCardId { get; set; }
        [JsonIgnore]
        public Guid WantedCardId { get; set; }

        [JsonConstructor]
        public PostTradeOfferRequestBody(string offeredCardId, string wantedCardId)
        {
            OfferedCardId = Guid.Parse(offeredCardId);
            WantedCardId = Guid.Parse(wantedCardId);
        }

    }
}
