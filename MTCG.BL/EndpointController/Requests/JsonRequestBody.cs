using Newtonsoft.Json;

namespace MTCG.BL.Requests
{
    public class JsonRequestBody<T>
    {
        public static T? FromJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
