using MTCG.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace MTCG.Serialization
{
    public class CardConverter : AbstractJsonConverter<Card>
    {
        protected override Card Create(Type objectType, JObject jObject)
        {
            if (FieldExists(jObject, "Race", JTokenType.Integer))
                return JsonConvert.DeserializeObject<MonsterCard>(jObject.ToString());
            else
                return JsonConvert.DeserializeObject<SpellCard>(jObject.ToString());
        }
    }
}
