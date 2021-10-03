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
            Card c = null;

            switch ((CardType)jObject.Property("CardType").Value.ToObject<int>())
            {
                case CardType.Monster:
                    c = JsonConvert.DeserializeObject<MonsterCard>(jObject.ToString());
                    break;
                default:
                case CardType.Spell:
                    c = JsonConvert.DeserializeObject<SpellCard>(jObject.ToString());
                    break;
            }

            var idObj = jObject.Property("ID")?.Value;
            if (idObj != null)
                c.SetID(Guid.Parse(idObj.ToString()));

            return c;
        }
    }
}
