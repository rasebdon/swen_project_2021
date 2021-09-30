using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MTCG.Http.Requests
{
    public class JsonRequestBody<T>
    {
        public static T FromJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
