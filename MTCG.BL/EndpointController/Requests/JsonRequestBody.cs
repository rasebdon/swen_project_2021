using Newtonsoft.Json;

namespace MTCG.BL.EndpointController.Requests
{
    public class JsonRequestBody<T>
    {
        public static T? FromJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
