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
            switch ((CardType)jObject.Property("CardType").Value.ToObject<int>())
            {
                case CardType.Monster:
                    return JsonConvert.DeserializeObject<MonsterCard>(jObject.ToString());
                default:
                case CardType.Spell:
                    return JsonConvert.DeserializeObject<SpellCard>(jObject.ToString());
            }
        }
    }
}
